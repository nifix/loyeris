namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// HTTP payload used to consume a reset token and replace an account password.
/// </summary>
public record ResetPasswordRequest(string Token, string NewPassword);
