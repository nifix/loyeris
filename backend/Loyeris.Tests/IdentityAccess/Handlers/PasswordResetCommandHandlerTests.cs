using FluentAssertions;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Handlers;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using Moq;

namespace Loyeris.Tests.IdentityAccess.Handlers;

/// <summary>
/// Ensures password reset requests and token consumption preserve account security.
/// </summary>
public class PasswordResetCommandHandlerTests
{
    /// <summary>
    /// Verifies malformed email addresses are rejected before any account lookup.
    /// </summary>
    [Test]
    public async Task RequestPasswordReset_ShouldReject_InvalidEmail()
    {
        // Arrange
        var users = new Mock<IAppUserAuthRepository>();
        var handler = CreateRequestHandler(users, new Mock<IPasswordResetSender>());

        // Act
        var result = await handler.Handle(
            new RequestPasswordResetCommand("invalid-email", null, null),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.password_reset.invalid_email");
        users.Verify(repo => repo.GetByNormalizedEmailAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies an unknown email receives the same successful response without sending a message.
    /// </summary>
    [Test]
    public async Task RequestPasswordReset_ShouldRemainNeutral_WhenAccountDoesNotExist()
    {
        // Arrange
        var users = new Mock<IAppUserAuthRepository>();
        users.Setup(repo => repo.GetByNormalizedEmailAsync("UNKNOWN@EXAMPLE.FR", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser)null!);
        var sender = new Mock<IPasswordResetSender>();
        var handler = CreateRequestHandler(users, sender: sender);

        // Act
        var result = await handler.Handle(
            new RequestPasswordResetCommand("unknown@example.fr", null, null),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(SuccessType.NoContent);
        sender.Verify(service => service.SendAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies a reset request replaces the active token and sends only its plain-text counterpart.
    /// </summary>
    [Test]
    public async Task RequestPasswordReset_ShouldRotateToken_AndSendEmail()
    {
        // Arrange
        var user = CreateActiveUser();
        var previousToken = CreateResetToken(user);
        AuthOneTimeToken? persistedToken = null;
        var persistenceSteps = new List<string>();

        var users = new Mock<IAppUserAuthRepository>();
        users.Setup(repo => repo.GetByNormalizedEmailAsync(user.NormalizedEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var tokens = new Mock<IAuthOneTimeTokenRepository>();
        tokens.Setup(repo => repo.GetActiveByUserAndPurposeAsync(
                user.Id, OneTimeTokenPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousToken);
        tokens.Setup(repo => repo.AddAsync(It.IsAny<AuthOneTimeToken>(), It.IsAny<CancellationToken>()))
            .Callback<AuthOneTimeToken, CancellationToken>((token, _) =>
            {
                persistenceSteps.Add("add");
                persistedToken = token;
            })
            .Returns(Task.CompletedTask);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Generate())
            .Returns(new GeneratedOneTimeToken("raw-reset-token", "RESET-TOKEN-HASH"));
        
        var sender = new Mock<IPasswordResetSender>();
        sender.Setup(service => service.SendAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var events = new Mock<IAuthEventRepository>();
        events.Setup(repo => repo.AddAsync(It.IsAny<AuthEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var unitOfWork = CreateTransactionalUnitOfWork(persistenceSteps);
        var handler = new RequestPasswordResetCommandHandler(
            users.Object,
            tokens.Object,
            events.Object,
            unitOfWork.Object,
            tokenService.Object,
            sender.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new RequestPasswordResetCommand(user.Email, "127.0.0.1", "Tests"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        previousToken.RevokedAt.Should().NotBeNull();
        persistedToken.Should().NotBeNull();
        persistedToken!.Purpose.Should().Be(OneTimeTokenPurpose.PasswordReset);
        persistedToken.TokenHash.Should().Be("RESET-TOKEN-HASH").And.NotBe("raw-reset-token");
        persistedToken.ExpiresAt.Should().BeCloseTo(DateTimeOffset.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
        persistenceSteps.Should().Equal("save", "add", "save");
        sender.Verify(service => service.SendAsync(
            user.Email, user.FirstName, "raw-reset-token", It.IsAny<CancellationToken>()), Times.Once);
        events.Verify(repo => repo.AddAsync(
            It.Is<AuthEvent>(authEvent => authEvent.Type == AuthEventType.PasswordResetRequested),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies expired password reset links are refused before the password form is displayed.
    /// </summary>
    [Test]
    public async Task ValidatePasswordResetToken_ShouldReject_ExpiredToken()
    {
        // Arrange
        var token = CreateResetToken(CreateActiveUser());
        token.ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1);
        
        var tokens = new Mock<IAuthOneTimeTokenRepository>();
        tokens.Setup(repo => repo.GetByTokenHashAsync("RESET-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("raw-token")).Returns("RESET-HASH");
        
        var handler = new ValidatePasswordResetTokenQueryHandler(
            tokens.Object,
            tokenService.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new ValidatePasswordResetTokenQuery("raw-token"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.password_reset.invalid_token");
    }

    /// <summary>
    /// Verifies a valid reset changes the password, consumes the token, and revokes active sessions.
    /// </summary>
    [Test]
    public async Task ResetPassword_ShouldChangePassword_AndRevokeSessions()
    {
        // Arrange
        var user = CreateActiveUser();
        var token = CreateResetToken(user);
        
        var session = new AuthSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = AuthSessionStatus.Active,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1)
        };
        
        var tokens = new Mock<IAuthOneTimeTokenRepository>();
        tokens.Setup(repo => repo.GetByTokenHashAsync("RESET-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("raw-token")).Returns("RESET-HASH");
        
        var sessions = new Mock<IAuthSessionRepository>();
        sessions.Setup(repo => repo.ListActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([session]);
        
        var passwordHasher = new Mock<IAccountPasswordHasher>();
        passwordHasher.Setup(hasher => hasher.HashPassword(user, "NewValid1!password"))
            .Returns("NEW-PASSWORD-HASH");
        
        var events = new Mock<IAuthEventRepository>();
        events.Setup(repo => repo.AddAsync(It.IsAny<AuthEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var unitOfWork = new Mock<IIdentityAccessUnitOfWork>();
        unitOfWork.Setup(unit => unit.TrySaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        
        var handler = new ResetPasswordCommandHandler(
            tokens.Object,
            Mock.Of<IAppUserAuthRepository>(),
            sessions.Object,
            events.Object,
            unitOfWork.Object,
            tokenService.Object,
            passwordHasher.Object,
            TimeProvider.System);
        var previousSecurityStamp = user.SecurityStamp;

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand("raw-token", "NewValid1!password", "127.0.0.1", "Tests"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        token.ConsumedAt.Should().NotBeNull();
        user.PasswordHash.Should().Be("NEW-PASSWORD-HASH");
        user.SecurityStamp.Should().NotBe(previousSecurityStamp);
        user.PasswordChangedAt.Should().NotBeNull();
        session.Status.Should().Be(AuthSessionStatus.Revoked);
        session.RevokedReason.Should().Be("password-reset");
        events.Verify(repo => repo.AddAsync(
            It.Is<AuthEvent>(authEvent => authEvent.Type == AuthEventType.PasswordResetCompleted),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies weak passwords are rejected before a reset token is queried.
    /// </summary>
    [Test]
    public async Task ResetPassword_ShouldReject_WeakPassword()
    {
        // Arrange
        var tokens = new Mock<IAuthOneTimeTokenRepository>();
        var handler = new ResetPasswordCommandHandler(
            tokens.Object,
            Mock.Of<IAppUserAuthRepository>(),
            Mock.Of<IAuthSessionRepository>(),
            Mock.Of<IAuthEventRepository>(),
            Mock.Of<IIdentityAccessUnitOfWork>(),
            Mock.Of<IOneTimeTokenService>(),
            Mock.Of<IAccountPasswordHasher>(),
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand("raw-token", "too-weak", null, null),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.password_reset.invalid_password");
        tokens.Verify(repo => repo.GetByTokenHashAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies an already consumed reset token produces the same refusal as any unusable link.
    /// </summary>
    [Test]
    public async Task ResetPassword_ShouldReject_ConsumedToken()
    {
        // Arrange
        var token = CreateResetToken(CreateActiveUser());
        token.ConsumedAt = DateTimeOffset.UtcNow.AddMinutes(-1);
        
        var tokens = new Mock<IAuthOneTimeTokenRepository>();
        tokens.Setup(repo => repo.GetByTokenHashAsync("RESET-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("raw-token")).Returns("RESET-HASH");
        
        var handler = new ResetPasswordCommandHandler(
            tokens.Object,
            Mock.Of<IAppUserAuthRepository>(),
            Mock.Of<IAuthSessionRepository>(),
            Mock.Of<IAuthEventRepository>(),
            Mock.Of<IIdentityAccessUnitOfWork>(),
            tokenService.Object,
            Mock.Of<IAccountPasswordHasher>(),
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand("raw-token", "NewValid1!password", null, null),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.password_reset.invalid_token");
    }

    private static RequestPasswordResetCommandHandler CreateRequestHandler(
        Mock<IAppUserAuthRepository> users,
        Mock<IPasswordResetSender> sender)
        => new(
            users.Object,
            Mock.Of<IAuthOneTimeTokenRepository>(),
            Mock.Of<IAuthEventRepository>(),
            Mock.Of<IIdentityAccessUnitOfWork>(),
            Mock.Of<IOneTimeTokenService>(),
            sender.Object,
            TimeProvider.System);

    private static Mock<IIdentityAccessUnitOfWork> CreateTransactionalUnitOfWork(List<string> persistenceSteps)
    {
        var unitOfWork = new Mock<IIdentityAccessUnitOfWork>();
        unitOfWork.Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => persistenceSteps.Add("save"))
            .ReturnsAsync(1);
        unitOfWork.Setup(unit => unit.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>(async (operation, cancellationToken) =>
            {
                await operation(cancellationToken);
                return true;
            });
        
        return unitOfWork;
    }

    private static AppUser CreateActiveUser()
        => new()
        {
            Id = Guid.NewGuid(),
            Email = "camille@example.fr",
            NormalizedEmail = "CAMILLE@EXAMPLE.FR",
            FirstName = "Camille",
            LastName = "Robert",
            PasswordHash = "OLD-PASSWORD-HASH",
            SecurityStamp = "old-security-stamp",
            Status = UserStatus.Active
        };

    private static AuthOneTimeToken CreateResetToken(AppUser user)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Purpose = OneTimeTokenPurpose.PasswordReset,
            TokenHash = "RESET-HASH",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };
}
