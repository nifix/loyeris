using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Provides persistence operations for hashed one-time authentication tokens.
/// </summary>
public interface IAuthOneTimeTokenRepository
{
    /// <summary>
    /// Finds a one-time token by its cryptographic hash.
    /// </summary>
    /// <param name="tokenHash">The token hash to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The matching one-time token, or null when no token exists.</returns>
    Task<AuthOneTimeToken> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>
    /// Finds the active one-time token for a user and purpose.
    /// </summary>
    /// <param name="userId">The owner user identifier.</param>
    /// <param name="purpose">The security workflow purpose.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The active one-time token, or null when none exists.</returns>
    Task<AuthOneTimeToken> GetActiveByUserAndPurposeAsync(
        Guid userId,
        OneTimeTokenPurpose purpose,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new hashed one-time token.
    /// </summary>
    /// <param name="oneTimeToken">The one-time token to add.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    Task AddAsync(AuthOneTimeToken oneTimeToken, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an existing one-time token as changed.
    /// </summary>
    /// <param name="oneTimeToken">The one-time token to update.</param>
    void Update(AuthOneTimeToken oneTimeToken);
}
