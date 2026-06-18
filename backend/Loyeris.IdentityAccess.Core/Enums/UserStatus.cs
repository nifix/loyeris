namespace Loyeris.IdentityAccess.Core.Enums;

/// <summary>
/// Describes the lifecycle state of an application user account.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// The user has been invited but has not completed account activation yet.
    /// </summary>
    Invited = 0,

    /// <summary>
    /// The user can authenticate and use the application.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The user account is blocked from accessing the application.
    /// </summary>
    Disabled = 2
}
