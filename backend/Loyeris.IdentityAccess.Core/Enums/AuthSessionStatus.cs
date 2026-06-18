namespace Loyeris.IdentityAccess.Core.Enums;

/// <summary>
/// Describes the lifecycle state of an authentication session.
/// </summary>
public enum AuthSessionStatus
{
    /// <summary>
    /// The session can still be used to refresh access tokens.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The session was explicitly revoked before its natural expiration.
    /// </summary>
    Revoked = 1,

    /// <summary>
    /// The session reached its maximum allowed lifetime.
    /// </summary>
    Expired = 2
}
