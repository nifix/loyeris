using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Revokes an authentication session and its current refresh token.
/// </summary>
public class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IAuthSessionRepository sessionRepository,
    IAuthEventRepository eventRepository,
    IIdentityAccessUnitOfWork unitOfWork,
    IOneTimeTokenService tokenService,
    TimeProvider timeProvider) : IRequestHandler<LogoutCommand, Result<Unit>>
{
    /// <inheritdoc />
    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<Unit>.NoContent();

        var refreshToken = await refreshTokenRepository.GetByTokenHashAsync(
            tokenService.Hash(request.RefreshToken),
            cancellationToken);

        if (refreshToken?.Session is null)
            return Result<Unit>.NoContent();

        var now = timeProvider.GetUtcNow();
        var session = refreshToken.Session;
        session.Status = AuthSessionStatus.Revoked;
        session.RevokedAt = now;
        session.RevokedReason = "logout";
        sessionRepository.Update(session);

        var activeToken = await refreshTokenRepository.GetActiveBySessionIdAsync(session.Id, cancellationToken);
        if (activeToken is not null)
        {
            activeToken.RevokedAt = now;
            activeToken.RevokedReason = "logout";
            refreshTokenRepository.Update(activeToken);
        }

        await eventRepository.AddAsync(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = session.UserId,
            SessionId = session.Id,
            NormalizedEmail = session.User?.NormalizedEmail,
            Type = AuthEventType.Logout,
            OccurredAt = now,
            IpAddress = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        }, cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Unit>.NoContent();
    }

    private static string Truncate(string value, int maximumLength)
        => string.IsNullOrEmpty(value) || value.Length <= maximumLength
            ? value
            : value[..maximumLength];
}
