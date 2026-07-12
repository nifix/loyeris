using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuthSessionRepository"/>.
/// </summary>
public class AuthSessionRepository(IdentityAccessDbContext dbContext) : IAuthSessionRepository
{
    /// <inheritdoc />
    public Task<AuthSession> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
        => dbContext.AuthSessions.SingleOrDefaultAsync(session => session.Id == sessionId, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuthSession>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.AuthSessions
            .Where(session => session.UserId == userId && session.Status == AuthSessionStatus.Active)
            .OrderByDescending(session => session.LastSeenAt ?? session.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(AuthSession session, CancellationToken cancellationToken)
        => await dbContext.AuthSessions.AddAsync(session, cancellationToken);

    /// <inheritdoc />
    public void Update(AuthSession session)
        => dbContext.AuthSessions.Update(session);
}
