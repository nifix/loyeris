using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRefreshTokenRepository"/>.
/// </summary>
public class RefreshTokenRepository(IdentityAccessDbContext dbContext) : IRefreshTokenRepository
{
    /// <inheritdoc />
    public Task<RefreshToken> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        => dbContext.RefreshTokens
            .Include(refreshToken => refreshToken.Session)
            .ThenInclude(session => session.User)
            .SingleOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash, cancellationToken);

    /// <inheritdoc />
    public Task<RefreshToken> GetActiveBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
        => dbContext.RefreshTokens.SingleOrDefaultAsync(
            refreshToken => refreshToken.SessionId == sessionId
                            && refreshToken.ConsumedAt == null
                            && refreshToken.RevokedAt == null,
            cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        => await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    /// <inheritdoc />
    public void Update(RefreshToken refreshToken)
        => dbContext.RefreshTokens.Update(refreshToken);
}
