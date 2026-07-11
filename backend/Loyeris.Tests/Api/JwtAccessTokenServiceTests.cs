using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Loyeris.Api.Security;
using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace Loyeris.Tests.Api;

/// <summary>
/// Ensures JWT access tokens contain the identity and session claims required by the API.
/// </summary>
public class JwtAccessTokenServiceTests
{
    /// <summary>
    /// Verifies the generated token carries the account and session identifiers without a refresh token.
    /// </summary>
    [Test]
    public void Generate_ShouldCreateSignedJwt_WithExpectedClaims()
    {
        // Arrange
        var options = Options.Create(new JwtOptions
        {
            Issuer = "Loyeris.Tests",
            Audience = "Loyeris.TestApp",
            SigningKey = "a-test-signing-key-with-at-least-thirty-two-bytes",
            AccessTokenLifetimeMinutes = 15
        });
        
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "camille@example.fr",
            FirstName = "Camille",
            LastName = "Robert",
            SecurityStamp = "security-stamp"
        };
        
        var sessionId = Guid.NewGuid();
        var service = new JwtAccessTokenService(options, TimeProvider.System);

        // Act
        var generated = service.Generate(user, sessionId);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(generated.Token);

        // Assert
        token.Issuer.Should().Be("Loyeris.Tests");
        token.Audiences.Should().ContainSingle("Loyeris.TestApp");
        token.Subject.Should().Be(user.Id.ToString());
        token.Claims.Should().Contain(claim => claim.Type == "sid" && claim.Value == sessionId.ToString());
        token.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == user.Email);
        token.Claims.Should().NotContain(claim => claim.Type.Contains("refresh", StringComparison.OrdinalIgnoreCase));
    }
}
