namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// HTTP payload used to open an authenticated browser session.
/// </summary>
public record LoginRequest(string Email, string Password, bool RememberMe);
