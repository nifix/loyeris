using FluentAssertions;
using Loyeris.Api.Configuration;
using Loyeris.Shared.Configuration;

namespace Loyeris.Tests.Api;

/// <summary>
/// Ensures operational configuration rejects incomplete or unsafe values.
/// </summary>
public class ConfigurationValidationTests
{
    /// <summary>
    /// Verifies the public frontend URL must use an absolute HTTP(S) address.
    /// </summary>
    [TestCase("https://loyeris.example.fr", true)]
    [TestCase("http://localhost:8080", true)]
    [TestCase("/relative", false)]
    [TestCase("ftp://loyeris.example.fr", false)]
    public void HasValidFrontendBaseUrl_ShouldValidate_PublicHttpOrigins(string value, bool expected)
    {
        // Arrange
        var options = new ApplicationUrlOptions { FrontendBaseUrl = value };

        // Act
        var result = ConfigurationValidation.HasValidFrontendBaseUrl(options);

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies SMTP authentication cannot be partially configured.
    /// </summary>
    [TestCase(null, null, true)]
    [TestCase("smtp-user", "smtp-password", true)]
    [TestCase("smtp-user", null, false)]
    [TestCase(null, "smtp-password", false)]
    public void HasMatchingSmtpCredentials_ShouldRequire_ACompletePair(
        string? username,
        string? password,
        bool expected)
    {
        // Arrange
        var options = new SmtpOptions { Username = username, Password = password };

        // Act
        var result = ConfigurationValidation.HasMatchingSmtpCredentials(options);

        // Assert
        result.Should().Be(expected);
    }
}
