using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Activates an account after successful one-time token verification.
/// </summary>
public class VerifyEmailCommandHandler(
    IAuthOneTimeTokenRepository tokenRepository,
    IAppUserAuthRepository userRepository,
    IAuthEventRepository authEventRepository,
    IIdentityAccessUnitOfWork unitOfWork,
    IOneTimeTokenService tokenService,
    TimeProvider timeProvider) : IRequestHandler<VerifyEmailCommand, Result<Unit>>
{
    private static readonly Error InvalidToken = new(
        "identity.email_verification.invalid_token",
        "Le lien de vérification est invalide ou a expiré.",
        ErrorType.Validation);

    /// <inheritdoc />
    public async Task<Result<Unit>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || request.Token.Length > 512)
            return Result<Unit>.Failure(InvalidToken);

        var tokenHash = tokenService.Hash(request.Token);
        var oneTimeToken = await tokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        var now = timeProvider.GetUtcNow();

        // Use one generic refusal for every invalid state to avoid disclosing token lifecycle details.
        if (oneTimeToken is null
            || oneTimeToken.Purpose != OneTimeTokenPurpose.EmailVerification
            || oneTimeToken.ExpiresAt <= now
            || oneTimeToken.ConsumedAt.HasValue
            || oneTimeToken.RevokedAt.HasValue
            || oneTimeToken.User is null
            || oneTimeToken.User.Status != UserStatus.Invited)
        {
            return Result<Unit>.Failure(InvalidToken);
        }

        // ConsumedAt is an EF concurrency token, so simultaneous requests cannot both consume this link.
        oneTimeToken.ConsumedAt = now;
        oneTimeToken.User.EmailConfirmedAt = now;
        oneTimeToken.User.Status = UserStatus.Active;
        oneTimeToken.User.UpdatedAt = now;
        tokenRepository.Update(oneTimeToken);
        userRepository.Update(oneTimeToken.User);
        await authEventRepository.AddAsync(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = oneTimeToken.UserId,
            NormalizedEmail = oneTimeToken.User.NormalizedEmail,
            Type = AuthEventType.EmailVerified,
            OccurredAt = now,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        }, cancellationToken);

        // A concurrency conflict is intentionally exposed as the same invalid-token result as a replay.
        return await unitOfWork.TrySaveChangesAsync(cancellationToken)
            ? Result<Unit>.NoContent()
            : Result<Unit>.Failure(InvalidToken);
    }
}
