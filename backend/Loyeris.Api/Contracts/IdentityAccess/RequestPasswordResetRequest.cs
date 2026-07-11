namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// HTTP payload used to request a password reset email.
/// </summary>
public record RequestPasswordResetRequest(string Email);
