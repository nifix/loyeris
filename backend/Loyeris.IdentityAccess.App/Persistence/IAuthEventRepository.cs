using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Provides persistence operations for authentication security events.
/// </summary>
public interface IAuthEventRepository
{
    /// <summary>
    /// Adds a new authentication security event.
    /// </summary>
    /// <param name="authEvent">The event to add.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    Task AddAsync(AuthEvent authEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Lists recent security events for a user.
    /// </summary>
    /// <param name="userId">The user identifier to search for.</param>
    /// <param name="count">The maximum number of events to return.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The recent security events for the user.</returns>
    Task<IReadOnlyList<AuthEvent>> ListRecentByUserIdAsync(Guid userId, int count, CancellationToken cancellationToken);
}
