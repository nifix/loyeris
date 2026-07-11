namespace Loyeris.IdentityAccess.App.Security;

/// <summary>
/// Contains a signed access token and its exact expiration time.
/// </summary>
public record GeneratedAccessToken(string Token, DateTimeOffset ExpiresAt);
