using Loyeris.IdentityAccess.App.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IIdentityAccessUnitOfWork"/>.
/// </summary>
public class IdentityAccessUnitOfWork(IdentityAccessDbContext dbContext) : IIdentityAccessUnitOfWork
{
    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }
}
