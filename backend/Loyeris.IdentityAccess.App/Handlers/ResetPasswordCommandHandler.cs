using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Consumes a password reset token, changes the password, and revokes existing sessions.
/// </summary>
public class ResetPasswordCommandHandler(
    IAuthOneTimeTokenRepository tokenRepository,
    IAppUserAuthRepository userRepository,
    IAuthSessionRepository sessionRepository,
    IAuthEventRepository eventRepository,
    IIdentityAccessUnitOfWork unitOfWork,
    IOneTimeTokenService tokenService,
    IAccountPasswordHasher passwordHasher,
    TimeProvider timeProvider) : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    private static readonly Error InvalidToken = new(
        "identity.password_reset.invalid_token",
        "Le lien de réinitialisation est invalide ou a expiré.",
        ErrorType.Validation);

    private static readonly Error InvalidPassword = new(
        "identity.password_reset.invalid_password",
        "Le nouveau mot de passe ne respecte pas les exigences de sécurité.",
        ErrorType.Validation);

    /// <inheritdoc />
    public async Task<Result<Unit>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (!AccountPasswordPolicy.IsValid(request.NewPassword))
            return Result<Unit>.Failure(InvalidPassword);

        if (string.IsNullOrWhiteSpace(request.Token) || request.Token.Length > 512)
            return Result<Unit>.Failure(InvalidToken);

        var oneTimeToken = await tokenRepository.GetByTokenHashAsync(
            tokenService.Hash(request.Token),
            cancellationToken);
        var now = timeProvider.GetUtcNow();

        if (!PasswordResetTokenPolicy.IsUsable(oneTimeToken, now))
            return Result<Unit>.Failure(InvalidToken);

        var user = oneTimeToken.User;
        var activeSessions = await sessionRepository.ListActiveByUserIdAsync(user.Id, cancellationToken);

        oneTimeToken.ConsumedAt = now;
        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.PasswordChangedAt = now;
        user.AccessFailedCount = 0;
        user.LockoutEndsAt = null;
        user.UpdatedAt = now;
        tokenRepository.Update(oneTimeToken);
        userRepository.Update(user);

        foreach (var session in activeSessions)
        {
            session.Status = AuthSessionStatus.Revoked;
            session.RevokedAt = now;
            session.RevokedReason = "password-reset";
            sessionRepository.Update(session);
        }

        await eventRepository.AddAsync(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            NormalizedEmail = user.NormalizedEmail,
            Type = AuthEventType.PasswordResetCompleted,
            OccurredAt = now,
            IpAddress = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        }, cancellationToken);

        // ConsumedAt is a concurrency token, so a reset link cannot be submitted successfully twice.
        return await unitOfWork.TrySaveChangesAsync(cancellationToken)
            ? Result<Unit>.NoContent()
            : Result<Unit>.Failure(InvalidToken);
    }

    private static string Truncate(string value, int maximumLength)
        => string.IsNullOrEmpty(value) || value.Length <= maximumLength
            ? value
            : value[..maximumLength];
}
