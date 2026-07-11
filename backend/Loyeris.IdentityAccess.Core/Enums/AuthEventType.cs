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
    AccountLocked = 6,

    /// <summary>
    /// A self-service account registration was completed.
    /// </summary>
    AccountRegistered = 7,

    /// <summary>
    /// A user confirmed ownership of their email address.
    /// </summary>
    EmailVerified = 8,

    /// <summary>
    /// A valid sign-in attempt triggered a replacement email verification link.
    /// </summary>
    EmailVerificationResent = 9
}
