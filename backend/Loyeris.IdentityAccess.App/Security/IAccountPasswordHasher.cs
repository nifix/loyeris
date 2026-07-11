using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Hashes account passwords without exposing the implementation to the application layer.
/// </summary>
public interface IAccountPasswordHasher
{
    /// <summary>
    /// Creates a password hash tied to the supplied user.
    /// </summary>
    string HashPassword(AppUser user, string password);

    /// <summary>
    /// Verifies a password against the hash stored for an account.
    /// </summary>
    bool VerifyPassword(AppUser user, string password);
}
