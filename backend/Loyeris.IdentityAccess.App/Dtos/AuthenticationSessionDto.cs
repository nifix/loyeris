namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Carries a new access token and the transient refresh token to the HTTP boundary.
/// </summary>
public record AuthenticationSessionDto(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    bool PersistentRefreshCookie,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName);
