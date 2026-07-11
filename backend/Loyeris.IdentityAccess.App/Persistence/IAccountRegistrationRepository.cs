using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Persists the complete account registration aggregate atomically.
/// </summary>
public interface IAccountRegistrationRepository
{
    /// <summary>
    /// Determines whether an account already uses the normalized email address.
    /// </summary>
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user and its complete registration graph in one database transaction.
    /// </summary>
    Task<AccountRegistrationPersistenceResult> CreateAsync(AppUser user, CancellationToken cancellationToken);
}
