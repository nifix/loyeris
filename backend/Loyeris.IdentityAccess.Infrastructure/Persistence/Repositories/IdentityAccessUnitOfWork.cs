using Loyeris.IdentityAccess.App.Persistence;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IIdentityAccessUnitOfWork"/>.
/// </summary>
public class IdentityAccessUnitOfWork(IdentityAccessDbContext dbContext) : IIdentityAccessUnitOfWork
{
    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
