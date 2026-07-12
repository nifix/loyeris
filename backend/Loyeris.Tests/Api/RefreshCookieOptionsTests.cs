using FluentAssertions;
using Loyeris.Api.Security;
using Microsoft.AspNetCore.Http;

namespace Loyeris.Tests.Api;

/// <summary>
/// Ensures refresh-token cookies honor the effective forwarded request scheme.
/// </summary>
public class RefreshCookieOptionsTests
{
    /// <summary>
    /// Verifies an HTTPS request produces a secure, HTTP-only refresh cookie.
    /// </summary>
    [Test]
    public void Create_ShouldSecureCookie_WhenEffectiveRequestUsesHttps()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = Uri.UriSchemeHttps;

        // Act
        var options = RefreshCookieOptions.Create(context);

        // Assert
        options.Secure.Should().BeTrue();
        options.HttpOnly.Should().BeTrue();
        options.SameSite.Should().Be(SameSiteMode.Strict);
        options.Path.Should().Be("/api/identity-access/auth");
    }

    /// <summary>
    /// Verifies local HTTP development does not mark the refresh cookie as secure.
    /// </summary>
    [Test]
    public void Create_ShouldAllowCookie_WhenLocalRequestUsesHttp()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = Uri.UriSchemeHttp;

        // Act
        var options = RefreshCookieOptions.Create(context);

        // Assert
        options.Secure.Should().BeFalse();
    }
}
