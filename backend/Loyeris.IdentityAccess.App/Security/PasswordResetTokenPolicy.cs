using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Centralizes the lifecycle checks required before a password reset token can be used.
/// </summary>
public static class PasswordResetTokenPolicy
{
    /// <summary>
    /// Determines whether a token belongs to an active account and can still be consumed.
    /// </summary>
    public static bool IsUsable(AuthOneTimeToken oneTimeToken, DateTimeOffset now)
    {
        return oneTimeToken is
               {
                   Purpose: OneTimeTokenPurpose.PasswordReset,
                   ConsumedAt: null,
                   RevokedAt: null,
                   User.Status: UserStatus.Active
               }
               && oneTimeToken.ExpiresAt > now;
    }
}
