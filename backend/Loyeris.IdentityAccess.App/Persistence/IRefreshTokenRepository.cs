using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Provides persistence operations for hashed refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Finds a refresh token by its cryptographic hash.
    /// </summary>
    /// <param name="tokenHash">The token hash to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The matching refresh token, or null when no token exists.</returns>
    Task<RefreshToken> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>
    /// Finds the currently active refresh token for a session.
    /// </summary>
    /// <param name="sessionId">The session identifier to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The active refresh token, or null when none exists.</returns>
    Task<RefreshToken> GetActiveBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new hashed refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to add.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an existing refresh token as changed.
    /// </summary>
    /// <param name="refreshToken">The refresh token to update.</param>
    void Update(RefreshToken refreshToken);
}
