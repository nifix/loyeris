namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Commits Identity Access persistence changes as a single unit.
/// </summary>
public interface IIdentityAccessUnitOfWork
{
    /// <summary>
    /// Saves pending changes to the underlying store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
