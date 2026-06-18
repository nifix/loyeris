using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Provides account lookups and mutations needed by authentication workflows.
/// </summary>
public interface IAppUserAuthRepository
{
    /// <summary>
    /// Finds a user by their technical identifier.
    /// </summary>
    /// <param name="userId">The user identifier to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The matching user, or null when no user exists.</returns>
    Task<AppUser> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Finds a user by normalized email for sign-in flows.
    /// </summary>
    /// <param name="normalizedEmail">The normalized email address to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The matching user, or null when no user exists.</returns>
    Task<AppUser> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user to the identity store.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    Task AddAsync(AppUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an existing user as changed.
    /// </summary>
    /// <param name="user">The user to update.</param>
    void Update(AppUser user);
}
