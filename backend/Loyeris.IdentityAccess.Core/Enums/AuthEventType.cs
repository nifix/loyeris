namespace Loyeris.IdentityAccess.Core.Enums;

/// <summary>
/// Describes the type of authentication security event.
/// </summary>
public enum AuthEventType
{
    /// <summary>
    /// A user successfully signed in.
    /// </summary>
    LoginSucceeded = 0,

    /// <summary>
    /// A sign-in attempt failed.
    /// </summary>
    LoginFailed = 1,

    /// <summary>
    /// A refresh token was rotated successfully.
    /// </summary>
    RefreshRotated = 2,

    /// <summary>
    /// A consumed or revoked refresh token was presented again.
    /// </summary>
    RefreshReuseDetected = 3,

    /// <summary>
    /// A session was logged out.
    /// </summary>
    Logout = 4,

    /// <summary>
    /// A user changed their password.
    /// </summary>
    PasswordChanged = 5,

    /// <summary>
    /// A user account was temporarily locked.
    /// </summary>
    AccountLocked = 6
}
