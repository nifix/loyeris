namespace Loyeris.Api.Security;

/// <summary>
/// Creates the security attributes shared by refresh-token cookie operations.
/// </summary>
public static class RefreshCookieOptions
{
    /// <summary>
    /// Creates cookie options from the effective request scheme after proxy forwarding.
    /// </summary>
    public static CookieOptions Create(HttpContext httpContext)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/api/identity-access/auth"
        };
    }
}
