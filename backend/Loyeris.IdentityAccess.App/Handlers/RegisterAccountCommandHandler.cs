using System.Net.Mail;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Creates the complete identity aggregate for a self-service registration.
/// </summary>
public class RegisterAccountCommandHandler(
    IAccountRegistrationRepository registrationRepository,
    IAccountPasswordHasher passwordHasher,
    IOneTimeTokenService tokenService,
    IEmailVerificationSender emailVerificationSender,
    TimeProvider timeProvider) : IRequestHandler<RegisterAccountCommand, Result<RegisteredAccountDto>>
{
    private static readonly Error InvalidRegistration = new(
        "identity.registration.invalid",
        "Les informations d'inscription sont invalides.",
        ErrorType.Validation);

    private static readonly Error DuplicateEmail = new(
        "identity.email_already_exists",
        "Un compte utilise déjà cette adresse email.",
        ErrorType.Conflict);

    /// <inheritdoc />
    public async Task<Result<RegisteredAccountDto>> Handle(
        RegisterAccountCommand request,
        CancellationToken cancellationToken)
    {
        // Normalize user-facing values once so validation, lookup, and persistence share the same input.
        var firstName = request.FirstName?.Trim();
        var lastName = request.LastName?.Trim();
        var email = request.Email?.Trim();

        if (!IsValid(firstName, lastName, email, request.Password, request.TermsAccepted))
        {
            return Result<RegisteredAccountDto>.Failure(InvalidRegistration);
        }

        var normalizedEmail = email.ToUpperInvariant();

        // This pre-check provides a fast conflict response; the database unique index still handles races.
        if (await registrationRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
            return Result<RegisteredAccountDto>.Failure(DuplicateEmail);

        var now = timeProvider.GetUtcNow();
        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var generatedToken = tokenService.Generate();

        // Build one connected aggregate so EF Core can persist the account and its defaults atomically.
        var user = new AppUser
        {
            Id = userId,
            Email = email,
            NormalizedEmail = normalizedEmail,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            FirstName = firstName,
            LastName = lastName,
            Status = UserStatus.Invited,
            PasswordChangedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        user.UserPreference = new UserPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            Language = "fr",
            Currency = "EUR",
            Theme = "corporate",
            CompactTables = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        var workspace = new Workspace
        {
            Id = workspaceId,
            Name = "Mon espace Loyeris",
            OwnerUserId = userId,
            OwnerUser = user,
            Status = WorkspaceStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };
        var membership = new WorkspaceMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            UserId = userId,
            Workspace = workspace,
            User = user,
            Role = WorkspaceRole.Owner,
            JoinedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        workspace.Members.Add(membership);
        user.OwnedWorkspaces.Add(workspace);
        user.WorkspaceMembers.Add(membership);
        user.AuthOneTimeTokens.Add(new AuthOneTimeToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            Purpose = OneTimeTokenPurpose.EmailVerification,
            TokenHash = generatedToken.Hash,
            SentToEmail = email,
            CreatedAt = now,
            ExpiresAt = now.AddHours(24),
            RequestedByIp = request.IpAddress,
            UserAgent = request.UserAgent
        });
        user.AuthEvents.Add(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            NormalizedEmail = normalizedEmail,
            Type = AuthEventType.AccountRegistered,
            OccurredAt = now,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        });

        var persistenceResult = await registrationRepository.CreateAsync(user, cancellationToken);

        if (persistenceResult == AccountRegistrationPersistenceResult.DuplicateEmail)
        {
            return Result<RegisteredAccountDto>.Failure(DuplicateEmail);
        }

        // The raw token only crosses the notification boundary; persistence receives its hash exclusively.
        await emailVerificationSender.SendAsync(email, firstName, generatedToken.PlainText, cancellationToken);

        return Result<RegisteredAccountDto>.Created(new RegisteredAccountDto(userId, email, true));
    }

    private static bool IsValid(
        string firstName,
        string lastName,
        string email,
        string password,
        bool termsAccepted)
        => termsAccepted
           && !string.IsNullOrWhiteSpace(firstName)
           && firstName.Length <= 120
           && !string.IsNullOrWhiteSpace(lastName)
           && lastName.Length <= 120
           && IsValidEmail(email)
           && AccountPasswordPolicy.IsValid(password);

    private static bool IsValidEmail(string email)
        => !string.IsNullOrWhiteSpace(email)
           && email.Length <= 320
           && MailAddress.TryCreate(email, out var parsed)
           && string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);

}
