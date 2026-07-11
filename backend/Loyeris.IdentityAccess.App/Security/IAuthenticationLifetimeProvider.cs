using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Provides configured authentication session lifetimes without coupling handlers to configuration.
/// </summary>
public interface IAuthenticationLifetimeProvider
{
    DateTimeOffset GetRefreshTokenExpiration(DateTimeOffset createdAt, bool persistent);
    bool UsesPersistentCookie(AuthSession session);
}
