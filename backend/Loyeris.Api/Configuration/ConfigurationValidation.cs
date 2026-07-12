using Loyeris.Shared.Configuration;

namespace Loyeris.Api.Configuration;

/// <summary>
/// Contains deterministic validation rules for operational configuration.
/// </summary>
public static class ConfigurationValidation
{
    /// <summary>
    /// Determines whether the public frontend URL is an absolute HTTP(S) URL.
    /// </summary>
    public static bool HasValidFrontendBaseUrl(ApplicationUrlOptions options)
    {
        return Uri.TryCreate(options.FrontendBaseUrl, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Determines whether the SMTP endpoint and sender identity are complete.
    /// </summary>
    public static bool HasValidSmtpEndpoint(SmtpOptions options)
    {
        return !string.IsNullOrWhiteSpace(options.Host)
               && options.Port is > 0 and <= 65535
               && !string.IsNullOrWhiteSpace(options.FromAddress);
    }

    /// <summary>
    /// Determines whether SMTP credentials are both configured or both omitted.
    /// </summary>
    public static bool HasMatchingSmtpCredentials(SmtpOptions options)
    {
        return string.IsNullOrWhiteSpace(options.Username)
               == string.IsNullOrWhiteSpace(options.Password);
    }
}
