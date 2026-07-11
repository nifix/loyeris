using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace Loyeris.Api.Security;

/// <summary>
/// Applies configured refresh-session lifetimes.
/// </summary>
public class AuthenticationLifetimeProvider(IOptions<JwtOptions> options) : IAuthenticationLifetimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset GetRefreshTokenExpiration(DateTimeOffset createdAt, bool persistent)
        => persistent
            ? createdAt.AddDays(options.Value.PersistentRefreshTokenLifetimeDays)
            : createdAt.AddHours(options.Value.RefreshTokenLifetimeHours);

    /// <inheritdoc />
    public bool UsesPersistentCookie(AuthSession session)
        => session.ExpiresAt - session.CreatedAt > TimeSpan.FromDays(2);
}
