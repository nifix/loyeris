namespace Loyeris.Shared.Configuration;

/// <summary>
/// Configures public application origins shared by link-producing workflows.
/// </summary>
public class ApplicationUrlOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "ApplicationUrls";

    /// <summary>
    /// Gets or sets the public frontend origin.
    /// </summary>
    public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
}
