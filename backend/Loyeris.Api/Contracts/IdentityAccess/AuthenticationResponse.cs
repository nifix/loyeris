namespace Loyeris.Api.Contracts.IdentityAccess;

/// <summary>
/// Public authentication response; the refresh token is deliberately excluded.
/// </summary>
public record AuthenticationResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName);
