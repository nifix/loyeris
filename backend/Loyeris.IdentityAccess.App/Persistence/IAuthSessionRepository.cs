using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Provides persistence operations for authentication sessions.
/// </summary>
public interface IAuthSessionRepository
{
    /// <summary>
    /// Finds a session by its technical identifier.
    /// </summary>
    /// <param name="sessionId">The session identifier to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The matching session, or null when no session exists.</returns>
    Task<AuthSession> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken);

    /// <summary>
    /// Lists the active sessions for a user, most recently seen first.
    /// </summary>
    /// <param name="userId">The owner user identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The active sessions for the user.</returns>
    Task<IReadOnlyList<AuthSession>> ListActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new authentication session.
    /// </summary>
    /// <param name="session">The session to add.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    Task AddAsync(AuthSession session, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an existing authentication session as changed.
    /// </summary>
    /// <param name="session">The session to update.</param>
    void Update(AuthSession session);
}
