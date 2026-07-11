using FluentAssertions;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Handlers;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Moq;

namespace Loyeris.Tests.IdentityAccess.Handlers;

/// <summary>
/// Ensures account registration and email verification commands enforce their security rules.
/// </summary>
public class AccountCommandHandlerTests
{
    /// <summary>
    /// Verifies <see cref="RegisterAccountCommandHandler"/> persists the complete pending account graph.
    /// </summary>
    [Test]
    public async Task RegisterAccount_ShouldCreate_CompletePendingAccountGraph()
    {
        // Arrange
        AppUser? persistedUser = null;

        var repository = new Mock<IAccountRegistrationRepository>();
        repository.Setup(repo => repo.EmailExistsAsync("CEDRIC@EXAMPLE.FR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(repo => repo.CreateAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()))
            .Callback<AppUser, CancellationToken>((user, _) => persistedUser = user)
            .ReturnsAsync(AccountRegistrationPersistenceResult.Created);

        var passwordHasher = new Mock<IAccountPasswordHasher>();
        passwordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<AppUser>(), "Valid1!password"))
            .Returns("hashed-password");

        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Generate())
            .Returns(new GeneratedOneTimeToken("raw-token", "TOKEN-HASH"));

        var sender = new Mock<IEmailVerificationSender>();
        sender.Setup(service => service.SendAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RegisterAccountCommandHandler(
            repository.Object,
            passwordHasher.Object,
            tokenService.Object,
            sender.Object,
            TimeProvider.System);

        var command = new RegisterAccountCommand(
            "  Cédric ", " Robert  ", " Cedric@Example.fr ", "Valid1!password", true, "127.0.0.1", "Tests");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(Loyeris.Shared.Results.SuccessType.Created);
        result.Value.Email.Should().Be("Cedric@Example.fr");
        result.Value.VerificationRequired.Should().BeTrue();

        persistedUser.Should().NotBeNull();
        persistedUser!.NormalizedEmail.Should().Be("CEDRIC@EXAMPLE.FR");
        persistedUser.PasswordHash.Should().Be("hashed-password").And.NotBe(command.Password);
        persistedUser.Status.Should().Be(UserStatus.Invited);
        persistedUser.EmailConfirmedAt.Should().BeNull();
        persistedUser.UserPreference.Language.Should().Be("fr");
        persistedUser.UserPreference.Currency.Should().Be("EUR");
        persistedUser.OwnedWorkspaces.Should().ContainSingle(workspace =>
            workspace.Name == "Mon espace Loyeris" && workspace.Status == WorkspaceStatus.Active);
        persistedUser.WorkspaceMembers.Should().ContainSingle(member => member.Role == WorkspaceRole.Owner);
        persistedUser.AuthOneTimeTokens.Should().ContainSingle(token =>
            token.TokenHash == "TOKEN-HASH"
            && token.TokenHash != "raw-token"
            && token.Purpose == OneTimeTokenPurpose.EmailVerification);
        persistedUser.AuthEvents.Should().ContainSingle(authEvent => authEvent.Type == AuthEventType.AccountRegistered);

        sender.Verify(service => service.SendAsync(
            "Cedric@Example.fr", "Cédric", "raw-token", It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies invalid registration data is rejected before persistence.
    /// </summary>
    [TestCase("", "Robert", "cedric@example.fr", "Valid1!password", true)]
    [TestCase("Cédric", "", "cedric@example.fr", "Valid1!password", true)]
    [TestCase("Cédric", "Robert", "invalid", "Valid1!password", true)]
    [TestCase("Cédric", "Robert", "cedric@example.fr", "short", true)]
    [TestCase("Cédric", "Robert", "cedric@example.fr", "lowercase1!", true)]
    [TestCase("Cédric", "Robert", "cedric@example.fr", "NoDigit!password", true)]
    [TestCase("Cédric", "Robert", "cedric@example.fr", "NoSpecial1password", true)]
    [TestCase("Cédric", "Robert", "cedric@example.fr", "Valid1!password", false)]
    public async Task RegisterAccount_ShouldReject_InvalidData(
        string firstName,
        string lastName,
        string email,
        string password,
        bool termsAccepted)
    {
        // Arrange
        var repository = new Mock<IAccountRegistrationRepository>();
        var handler = new RegisterAccountCommandHandler(
            repository.Object,
            Mock.Of<IAccountPasswordHasher>(),
            Mock.Of<IOneTimeTokenService>(),
            Mock.Of<IEmailVerificationSender>(),
            TimeProvider.System);
        var command = new RegisterAccountCommand(
            firstName, lastName, email, password, termsAccepted, "127.0.0.1", "Tests");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.registration.invalid");
        repository.Verify(repo => repo.CreateAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies duplicate emails are returned as a stable conflict result.
    /// </summary>
    [Test]
    public async Task RegisterAccount_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var repository = new Mock<IAccountRegistrationRepository>();
        repository.Setup(repo => repo.EmailExistsAsync("CEDRIC@EXAMPLE.FR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new RegisterAccountCommandHandler(
            repository.Object,
            Mock.Of<IAccountPasswordHasher>(),
            Mock.Of<IOneTimeTokenService>(),
            Mock.Of<IEmailVerificationSender>(),
            TimeProvider.System);

        // Act
        var result = await handler.Handle(new RegisterAccountCommand(
            "Cédric", "Robert", "cedric@example.fr", "Valid1!password", true, null, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.email_already_exists");
        result.Error.Type.Should().Be(Loyeris.Shared.Results.ErrorType.Conflict);
    }

    /// <summary>
    /// Verifies <see cref="VerifyEmailCommandHandler"/> consumes the token and activates its user.
    /// </summary>
    [Test]
    public async Task VerifyEmail_ShouldConsumeToken_AndActivateUser()
    {
        // Arrange
        var token = CreateVerificationToken();
        var tokenRepository = new Mock<IAuthOneTimeTokenRepository>();
        tokenRepository.Setup(repo => repo.GetByTokenHashAsync("TOKEN-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var eventRepository = new Mock<IAuthEventRepository>();
        eventRepository.Setup(repo => repo.AddAsync(It.IsAny<AuthEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var unitOfWork = new Mock<IIdentityAccessUnitOfWork>();
        unitOfWork.Setup(unit => unit.TrySaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("raw-token")).Returns("TOKEN-HASH");
        var handler = new VerifyEmailCommandHandler(
            tokenRepository.Object,
            Mock.Of<IAppUserAuthRepository>(),
            eventRepository.Object,
            unitOfWork.Object,
            tokenService.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new VerifyEmailCommand("raw-token", "127.0.0.1", "Tests"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(Loyeris.Shared.Results.SuccessType.NoContent);
        token.ConsumedAt.Should().NotBeNull();
        token.User.Status.Should().Be(UserStatus.Active);
        token.User.EmailConfirmedAt.Should().NotBeNull();
        eventRepository.Verify(repo => repo.AddAsync(
            It.Is<AuthEvent>(authEvent => authEvent.Type == AuthEventType.EmailVerified),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies expired, revoked, and consumed tokens produce the same refusal.
    /// </summary>
    [TestCase("expired")]
    [TestCase("revoked")]
    [TestCase("consumed")]
    public async Task VerifyEmail_ShouldReject_UnusableToken(string state)
    {
        // Arrange
        var token = CreateVerificationToken();
        var now = DateTimeOffset.UtcNow;
        token.ExpiresAt = state == "expired" ? now.AddMinutes(-1) : now.AddHours(1);
        token.RevokedAt = state == "revoked" ? now : null;
        token.ConsumedAt = state == "consumed" ? now : null;
        var tokenRepository = new Mock<IAuthOneTimeTokenRepository>();
        tokenRepository.Setup(repo => repo.GetByTokenHashAsync("TOKEN-HASH", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var tokenService = new Mock<IOneTimeTokenService>();
        tokenService.Setup(service => service.Hash("raw-token")).Returns("TOKEN-HASH");
        var handler = new VerifyEmailCommandHandler(
            tokenRepository.Object,
            Mock.Of<IAppUserAuthRepository>(),
            Mock.Of<IAuthEventRepository>(),
            Mock.Of<IIdentityAccessUnitOfWork>(),
            tokenService.Object,
            TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new VerifyEmailCommand("raw-token", null, null),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.email_verification.invalid_token");
    }

    private static AuthOneTimeToken CreateVerificationToken()
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "cedric@example.fr",
            NormalizedEmail = "CEDRIC@EXAMPLE.FR",
            Status = UserStatus.Invited
        };

        return new AuthOneTimeToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Purpose = OneTimeTokenPurpose.EmailVerification,
            TokenHash = "TOKEN-HASH",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };
    }
}
