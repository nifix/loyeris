namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Defines the password rules shared by account registration and password reset workflows.
/// </summary>
public static class AccountPasswordPolicy
{
    /// <summary>
    /// Determines whether a password satisfies the account security requirements.
    /// </summary>
    public static bool IsValid(string password)
    {
        return password is { Length: >= 8 and <= 128 }
               && password.Any(char.IsUpper)
               && password.Any(char.IsDigit)
               && password.Any(character => !char.IsLetterOrDigit(character));
    }
}
