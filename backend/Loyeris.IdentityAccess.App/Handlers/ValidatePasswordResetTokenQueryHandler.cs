using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Validates password reset links without consuming their one-time token.
/// </summary>
public class ValidatePasswordResetTokenQueryHandler(
    IAuthOneTimeTokenRepository tokenRepository,
    IOneTimeTokenService tokenService,
    TimeProvider timeProvider) : IRequestHandler<ValidatePasswordResetTokenQuery, Result<Unit>>
{
    private static readonly Error InvalidToken = new(
        "identity.password_reset.invalid_token",
        "Le lien de réinitialisation est invalide ou a expiré.",
        ErrorType.Validation);

    /// <inheritdoc />
    public async Task<Result<Unit>> Handle(
        ValidatePasswordResetTokenQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || request.Token.Length > 512)
            return Result<Unit>.Failure(InvalidToken);

        var oneTimeToken = await tokenRepository.GetByTokenHashAsync(
            tokenService.Hash(request.Token),
            cancellationToken);

        return PasswordResetTokenPolicy.IsUsable(oneTimeToken, timeProvider.GetUtcNow())
            ? Result<Unit>.NoContent()
            : Result<Unit>.Failure(InvalidToken);
    }
}
