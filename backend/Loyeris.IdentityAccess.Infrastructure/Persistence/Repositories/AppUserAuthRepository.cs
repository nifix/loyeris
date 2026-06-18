using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAppUserAuthRepository"/>.
/// </summary>
public class AppUserAuthRepository(IdentityAccessDbContext dbContext) : IAppUserAuthRepository
{
    /// <inheritdoc />
    public Task<AppUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        => dbContext.AppUsers.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);

    /// <inheritdoc />
    public Task<AppUser> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        => dbContext.AppUsers.SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(AppUser user, CancellationToken cancellationToken)
        => await dbContext.AppUsers.AddAsync(user, cancellationToken);

    /// <inheritdoc />
    public void Update(AppUser user)
        => dbContext.AppUsers.Update(user);
}
