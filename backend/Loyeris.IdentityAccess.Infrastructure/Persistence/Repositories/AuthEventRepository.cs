using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAuthEventRepository"/>.
/// </summary>
public class AuthEventRepository(IdentityAccessDbContext dbContext) : IAuthEventRepository
{
    /// <inheritdoc />
    public async Task AddAsync(AuthEvent authEvent, CancellationToken cancellationToken)
        => await dbContext.AuthEvents.AddAsync(authEvent, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuthEvent>> ListRecentByUserIdAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken)
    {
        return await dbContext.AuthEvents
            .Where(authEvent => authEvent.UserId == userId)
            .OrderByDescending(authEvent => authEvent.OccurredAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
