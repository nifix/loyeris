namespace Loyeris.Shared.Configuration;

/// <summary>
/// Configures the shared SMTP delivery server and sender identity.
/// </summary>
public class SmtpOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "Smtp";

    /// <summary>
    /// Gets or sets the SMTP server hostname.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
    public int Port { get; set; } = 1025;

    /// <summary>
    /// Gets or sets whether TLS must be enabled for SMTP transport.
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Gets or sets the sender email address.
    /// </summary>
    public string FromAddress { get; set; } = "no-reply@loyeris.local";

    /// <summary>
    /// Gets or sets the sender display name.
    /// </summary>
    public string FromName { get; set; } = "Loyeris";

    /// <summary>
    /// Gets or sets the optional SMTP username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the optional SMTP password.
    /// </summary>
    public string Password { get; set; }
}
