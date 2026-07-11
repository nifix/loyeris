namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Contains the transient raw token and the hash safe to persist.
/// </summary>
public record GeneratedOneTimeToken(string PlainText, string Hash);
