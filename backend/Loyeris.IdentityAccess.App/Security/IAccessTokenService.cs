using Loyeris.IdentityAccess.Core.Entities;

namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Issues signed access tokens for authenticated sessions.
/// </summary>
public interface IAccessTokenService
{
    GeneratedAccessToken Generate(AppUser user, Guid sessionId);
}
