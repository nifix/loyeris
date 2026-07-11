namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// HTTP payload used to validate a password reset link without consuming it.
/// </summary>
public record ValidatePasswordResetTokenRequest(string Token);
