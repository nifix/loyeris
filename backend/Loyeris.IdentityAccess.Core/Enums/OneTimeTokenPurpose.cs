namespace Loyeris.IdentityAccess.Core.Enums;

/// <summary>
/// Describes the security workflow a one-time token is issued for.
/// </summary>
public enum OneTimeTokenPurpose
{
    /// <summary>
    /// The token confirms ownership of an email address.
    /// </summary>
    EmailVerification = 0,

    /// <summary>
    /// The token authorizes a password reset.
    /// </summary>
    PasswordReset = 1,

    /// <summary>
    /// The token accepts a pending workspace or account invitation.
    /// </summary>
    InviteAcceptance = 2
}
