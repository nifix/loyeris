namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Describes a newly registered account waiting for email verification.
/// </summary>
public record RegisteredAccountDto(Guid UserId, string Email, bool VerificationRequired);
