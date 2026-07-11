namespace Loyeris.Shared.Configuration;

/// <summary>
/// Configures JWT access tokens, refresh sessions, and the refresh cookie.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SigningKey { get; set; }
    public int AccessTokenLifetimeMinutes { get; set; } = 15;
    public int RefreshTokenLifetimeHours { get; set; } = 24;
    public int PersistentRefreshTokenLifetimeDays { get; set; } = 30;
    public string RefreshCookieName { get; set; } = "loyeris.refresh_token";
}
