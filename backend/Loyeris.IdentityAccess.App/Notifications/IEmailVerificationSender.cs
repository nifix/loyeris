namespace Loyeris.IdentityAccess.App.Notifications;

/// <summary>
/// Delivers an email verification token through the configured notification channel.
/// </summary>
public interface IEmailVerificationSender
{
    /// <summary>
    /// Sends the verification instructions for a newly created account.
    /// </summary>
    Task SendAsync(string email, string firstName, string plainTextToken, CancellationToken cancellationToken);
}
