namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// HTTP payload used to create a self-service account.
/// </summary>
public record RegisterAccountRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    bool TermsAccepted);
