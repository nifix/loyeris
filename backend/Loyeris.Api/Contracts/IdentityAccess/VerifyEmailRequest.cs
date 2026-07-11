namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// HTTP payload used to consume an email verification token.
/// </summary>
public record VerifyEmailRequest(string Token);
