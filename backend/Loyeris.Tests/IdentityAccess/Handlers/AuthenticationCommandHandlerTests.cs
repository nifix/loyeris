using FluentAssertions;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Handlers;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using Moq;

namespace Loyeris.Tests.IdentityAccess.Handlers;

/// <summary>
/// Ensures login and refresh commands enforce the authentication session lifecycle.
/// </summary>
public class AuthenticationCommandHandlerTests
{
    /// <summary>
    /// Verifies a valid <see cref="LoginCommand"/> creates a session and returns access credentials.
    /// </summary>
    [Test]
    public async Task Login_ShouldCreateSession_WhenCredentialsAreValid()
    {
        // Arrange
        var user = CreateUser(UserStatus.Active);
        
        AuthSession? persistedSession = null;
        
        var users = new Mock<IAppUserAuthRepository>();
        users.Setup(repo => repo.GetByNormalizedEmailAsync("CAMILLE@EXAMPLE.FR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var sessions = new Mock<IAuthSessionRepository>();
        sessions.Setup(repo => repo.AddAsync(It.IsAny<AuthSession>(), It.IsAny<CancellationToken>()))
            .Callback<AuthSession, CancellationToken>((session, _) => persistedSession = session)
            .Returns(Task.CompletedTask);
        
        var passwordHasher = new Mock<IAccountPasswordHasher>();
        passwordHasher.Setup(hasher => hasher.VerifyPassword(user, "Valid1!password")).Returns(true);
        
        var tokens = new Mock<IOneTimeTokenService>();
        tokens.Setup(service => service.Generate()).Returns(new GeneratedOneTimeToken("refresh-raw", "REFRESH-HASH"));
        
        var accessTokens = new Mock<IAccessTokenService>();
        accessTokens.Setup(service => service.Generate(user, It.IsAny<Guid>()))
            .Returns(new GeneratedAccessToken("jwt-access", DateTimeOffset.UtcNow.AddMinutes(15)));
        
        var lifetimes = new Mock<IAuthenticationLifetimeProvider>();
        lifetimes.Setup(provider => provider.GetRefreshTokenExpiration(It.IsAny<DateTimeOffset>(), true))
            .Returns(DateTimeOffset.UtcNow.AddDays(30));
        
        var handler = CreateLoginHandler(
            users,
            sessions,
            passwordHasher,
            tokens,
            accessTokens,
            lifetimes);

        // Act
        var result = await handler.Handle(new LoginCommand(
            "camille@example.fr", "Valid1!password", true, "127.0.0.1", "Tests"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("jwt-access");
        result.Value.RefreshToken.Should().Be("refresh-raw");
        result.Value.PersistentRefreshCookie.Should().BeTrue();
        persistedSession.Should().NotBeNull();
        persistedSession!.RefreshTokens.Should().ContainSingle(token => token.TokenHash == "REFRESH-HASH");
        persistedSession.AuthEvents.Should().ContainSingle(authEvent => authEvent.Type == AuthEventType.LoginSucceeded);
        user.LastLoginAt.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies a valid password for an unverified account rotates its verification token without issuing a JWT.
    /// </summary>
    [Test]
    public async Task Login_ShouldResendVerification_WhenEmailIsNotVerified()
    {
        // Arrange
        var user = CreateUser(UserStatus.Invited);
        var oldToken = new AuthOneTimeToken { Id = Guid.NewGuid(), UserId = user.Id };
        
        var users = new Mock<IAppUserAuthRepository>();
        users.Setup(repo => repo.GetByNormalizedEmailAsync(user.NormalizedEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var passwordHasher = new Mock<IAccountPasswordHasher>();
        passwordHasher.Setup(hasher => hasher.VerifyPassword(user, "Valid1!password")).Returns(true);
        
        var oneTimeTokens = new Mock<IAuthOneTimeTokenRepository>();
        oneTimeTokens.Setup(repo => repo.GetActiveByUserAndPurposeAsync(
                user.Id, OneTimeTokenPurpose.EmailVerification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldToken);
        oneTimeTokens.Setup(repo => repo.AddAsync(It.IsAny<AuthOneTimeToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Generate())
            .Returns(new GeneratedOneTimeToken("new-verification-token", "NEW-HASH"));
        
        var sender = new Mock<IEmailVerificationSender>();
        sender.Setup(service => service.SendAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var events = new Mock<IAuthEventRepository>();
        events.Setup(repo => repo.AddAsync(It.IsAny<AuthEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var handler = new LoginCommandHandler(
            users.Object,
            Mock.Of<IAuthSessionRepository>(),
            oneTimeTokens.Object,
            events.Object,
            Mock.Of<IIdentityAccessUnitOfWork>(),
            passwordHasher.Object,
            tokenService.Object,
            Mock.Of<IAccessTokenService>(),
            Mock.Of<IAuthenticationLifetimeProvider>(),
            sender.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(new LoginCommand(
            user.Email, "Valid1!password", false, null, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.email_not_verified");
        oldToken.RevokedAt.Should().NotBeNull();
        oneTimeTokens.Verify(repo => repo.AddAsync(
            It.Is<AuthOneTimeToken>(token => token.TokenHash == "NEW-HASH"),
            It.IsAny<CancellationToken>()), Times.Once);
        sender.Verify(service => service.SendAsync(
            user.Email, user.FirstName, "new-verification-token", It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies refresh tokens are consumed and replaced atomically.
    /// </summary>
    [Test]
    public async Task Refresh_ShouldRotateToken_AndIssueNewAccessToken()
    {
        // Arrange
        var user = CreateUser(UserStatus.Active);
        var session = CreateSession(user);
        
        var currentToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            Session = session,
            TokenHash = "OLD-HASH",
            ExpiresAt = session.ExpiresAt
        };
        
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        refreshTokens.Setup(repo => repo.GetByTokenHashAsync("OLD-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentToken);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("old-raw")).Returns("OLD-HASH");
        tokenService.Setup(service => service.Generate()).Returns(new GeneratedOneTimeToken("new-raw", "NEW-HASH"));
        
        var accessTokens = new Mock<IAccessTokenService>();
        accessTokens.Setup(service => service.Generate(user, session.Id))
            .Returns(new GeneratedAccessToken("new-jwt", DateTimeOffset.UtcNow.AddMinutes(15)));
        
        var persistenceSteps = new List<string>();
        
        var unitOfWork = new Mock<IIdentityAccessUnitOfWork>();
        unitOfWork.Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => persistenceSteps.Add("save"))
            .ReturnsAsync(1);
        unitOfWork.Setup(unit => unit.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>(async (operation, cancellationToken) =>
            {
                await operation(cancellationToken);
                return true;
            });
        
        refreshTokens.Setup(repo => repo.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback(() => persistenceSteps.Add("add-replacement"))
            .Returns(Task.CompletedTask);
        
        var events = new Mock<IAuthEventRepository>();
        events.Setup(repo => repo.AddAsync(It.IsAny<AuthEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var handler = new RefreshSessionCommandHandler(
            refreshTokens.Object,
            Mock.Of<IAuthSessionRepository>(),
            events.Object,
            unitOfWork.Object,
            tokenService.Object,
            accessTokens.Object,
            Mock.Of<IAuthenticationLifetimeProvider>(),
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new RefreshSessionCommand("old-raw", "127.0.0.1", "Tests"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("new-jwt");
        result.Value.RefreshToken.Should().Be("new-raw");
        currentToken.ConsumedAt.Should().NotBeNull();
        currentToken.ReplacedByTokenId.Should().NotBeNull();
        persistenceSteps.Should().Equal("save", "add-replacement", "save");
        refreshTokens.Verify(repo => repo.AddAsync(
            It.Is<RefreshToken>(token => token.TokenHash == "NEW-HASH"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies presenting a consumed refresh token revokes the entire session.
    /// </summary>
    [Test]
    public async Task Refresh_ShouldRevokeSession_WhenConsumedTokenIsReused()
    {
        // Arrange
        var user = CreateUser(UserStatus.Active);
        var session = CreateSession(user);
        var consumedToken = new RefreshToken
        {
            SessionId = session.Id,
            Session = session,
            ConsumedAt = DateTimeOffset.UtcNow.AddMinutes(-1)
        };
        
        var activeToken = new RefreshToken { SessionId = session.Id };
        
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        refreshTokens.Setup(repo => repo.GetByTokenHashAsync("REUSED-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(consumedToken);
        refreshTokens.Setup(repo => repo.GetActiveBySessionIdAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeToken);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("reused-raw")).Returns("REUSED-HASH");
        
        var events = new Mock<IAuthEventRepository>();
        events.Setup(repo => repo.AddAsync(It.IsAny<AuthEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var handler = new RefreshSessionCommandHandler(
            refreshTokens.Object,
            Mock.Of<IAuthSessionRepository>(),
            events.Object,
            Mock.Of<IIdentityAccessUnitOfWork>(),
            tokenService.Object,
            Mock.Of<IAccessTokenService>(),
            Mock.Of<IAuthenticationLifetimeProvider>(),
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new RefreshSessionCommand("reused-raw", null, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        session.Status.Should().Be(AuthSessionStatus.Revoked);
        session.RevokedReason.Should().Be("refresh-token-reuse");
        activeToken.RevokedAt.Should().NotBeNull();
        events.Verify(repo => repo.AddAsync(
            It.Is<AuthEvent>(authEvent => authEvent.Type == AuthEventType.RefreshReuseDetected),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static LoginCommandHandler CreateLoginHandler(
        Mock<IAppUserAuthRepository> users,
        Mock<IAuthSessionRepository> sessions,
        Mock<IAccountPasswordHasher> passwordHasher,
        Mock<IOneTimeTokenService> tokens,
        Mock<IAccessTokenService> accessTokens,
        Mock<IAuthenticationLifetimeProvider> lifetimes)
    {
        return new LoginCommandHandler(
            users.Object,
            sessions.Object,
            Mock.Of<IAuthOneTimeTokenRepository>(),
            Mock.Of<IAuthEventRepository>(),
            Mock.Of<IIdentityAccessUnitOfWork>(),
            passwordHasher.Object,
            tokens.Object,
            accessTokens.Object,
            lifetimes.Object,
            Mock.Of<IEmailVerificationSender>(),
            TimeProvider.System);
    }

    private static AppUser CreateUser(UserStatus status)
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "camille@example.fr",
            NormalizedEmail = "CAMILLE@EXAMPLE.FR",
            PasswordHash = "HASH",
            SecurityStamp = "security-stamp",
            FirstName = "Camille",
            LastName = "Robert",
            Status = status
        };
    }

    private static AuthSession CreateSession(AppUser user)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Status = AuthSessionStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1)
        };
    }
}
