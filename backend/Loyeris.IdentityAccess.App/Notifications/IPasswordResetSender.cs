namespace Loyeris.IdentityAccess.App.Notifications;

/// <summary>
/// Delivers password reset instructions through the configured notification channel.
/// </summary>
public interface IPasswordResetSender
{
    /// <summary>
    /// Sends a one-time password reset link to an account owner.
    /// </summary>
    Task SendAsync(string email, string firstName, string plainTextToken, CancellationToken cancellationToken);
}
