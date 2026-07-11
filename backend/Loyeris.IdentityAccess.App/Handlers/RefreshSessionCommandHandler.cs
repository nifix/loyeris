using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Rotates refresh tokens and revokes sessions when reuse is detected.
/// </summary>
public class RefreshSessionCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IAuthSessionRepository sessionRepository,
    IAuthEventRepository eventRepository,
    IIdentityAccessUnitOfWork unitOfWork,
    IOneTimeTokenService tokenService,
    IAccessTokenService accessTokenService,
    IAuthenticationLifetimeProvider lifetimeProvider,
    TimeProvider timeProvider) : IRequestHandler<RefreshSessionCommand, Result<AuthenticationSessionDto>>
{
    private static readonly Error InvalidRefreshToken = new(
        "identity.invalid_refresh_token",
        "La session ne peut pas être renouvelée.",
        ErrorType.Unauthorized);

    /// <inheritdoc />
    public async Task<Result<AuthenticationSessionDto>> Handle(
        RefreshSessionCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken) || request.RefreshToken.Length > 512)
            return Result<AuthenticationSessionDto>.Failure(InvalidRefreshToken);

        var tokenHash = tokenService.Hash(request.RefreshToken);
        var currentToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        var now = timeProvider.GetUtcNow();

        if (currentToken?.Session?.User is null)
            return Result<AuthenticationSessionDto>.Failure(InvalidRefreshToken);

        var session = currentToken.Session;
        if (currentToken.ConsumedAt.HasValue || currentToken.RevokedAt.HasValue)
        {
            await RevokeForReuseAsync(session, request, now, cancellationToken);
            return Result<AuthenticationSessionDto>.Failure(InvalidRefreshToken);
        }

        if (currentToken.ExpiresAt <= now
            || session.ExpiresAt <= now
            || session.Status != AuthSessionStatus.Active
            || session.RevokedAt.HasValue
            || session.User.Status != UserStatus.Active)
        {
            return Result<AuthenticationSessionDto>.Failure(InvalidRefreshToken);
        }

        var replacement = tokenService.Generate();
        var replacementId = Guid.NewGuid();
        var replacementEntity = new RefreshToken
        {
            Id = replacementId,
            SessionId = session.Id,
            Session = session,
            TokenHash = replacement.Hash,
            CreatedAt = now,
            ExpiresAt = session.ExpiresAt,
            CreatedByIp = Truncate(request.IpAddress, 64)
        };

        currentToken.ConsumedAt = now;
        currentToken.ConsumedByIp = Truncate(request.IpAddress, 64);

        session.LastSeenAt = now;
        refreshTokenRepository.Update(currentToken);
        sessionRepository.Update(session);

        var rotationSucceeded = await unitOfWork.ExecuteInTransactionAsync(async transactionCancellationToken =>
        {
            // The old token must stop matching the unique active-token index before its replacement is inserted.
            await unitOfWork.SaveChangesAsync(transactionCancellationToken);

            currentToken.ReplacedByTokenId = replacementId;
            currentToken.ReplacedByToken = replacementEntity;
            await refreshTokenRepository.AddAsync(replacementEntity, transactionCancellationToken);
            
            await eventRepository.AddAsync(new AuthEvent
            {
                Id = Guid.NewGuid(),
                UserId = session.UserId,
                SessionId = session.Id,
                NormalizedEmail = session.User.NormalizedEmail,
                Type = AuthEventType.RefreshRotated,
                OccurredAt = now,
                IpAddress = Truncate(request.IpAddress, 64),
                UserAgent = Truncate(request.UserAgent, 512)
            }, transactionCancellationToken);

            await unitOfWork.SaveChangesAsync(transactionCancellationToken);
        }, cancellationToken);

        if (!rotationSucceeded)
            return Result<AuthenticationSessionDto>.Failure(InvalidRefreshToken);

        var accessToken = accessTokenService.Generate(session.User, session.Id);
        return Result<AuthenticationSessionDto>.Success(new AuthenticationSessionDto(
            accessToken.Token,
            accessToken.ExpiresAt,
            replacement.PlainText,
            replacementEntity.ExpiresAt,
            lifetimeProvider.UsesPersistentCookie(session),
            session.User.Id,
            session.User.Email,
            session.User.FirstName,
            session.User.LastName));
    }

    private async Task RevokeForReuseAsync(
        AuthSession session,
        RefreshSessionCommand request,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        session.Status = AuthSessionStatus.Revoked;
        session.RevokedAt = now;
        session.RevokedReason = "refresh-token-reuse";
        sessionRepository.Update(session);

        var activeToken = await refreshTokenRepository.GetActiveBySessionIdAsync(session.Id, cancellationToken);
        if (activeToken is not null)
        {
            activeToken.RevokedAt = now;
            activeToken.RevokedReason = "refresh-token-reuse";
            refreshTokenRepository.Update(activeToken);
        }

        await eventRepository.AddAsync(new AuthEvent
        {
            Id = Guid.NewGuid(),
            UserId = session.UserId,
            SessionId = session.Id,
            NormalizedEmail = session.User.NormalizedEmail,
            Type = AuthEventType.RefreshReuseDetected,
            OccurredAt = now,
            IpAddress = Truncate(request.IpAddress, 64),
            UserAgent = Truncate(request.UserAgent, 512)
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string Truncate(string value, int maximumLength)
        => string.IsNullOrEmpty(value) || value.Length <= maximumLength
            ? value
            : value[..maximumLength];
}
