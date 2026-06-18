namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Stores per-user interface preferences that do not belong to a specific workspace.
/// </summary>
public class UserPreference
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the preference row.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user these preferences belong to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the preferred UI language code.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the preferred currency code used for formatting amounts.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the preferred visual theme.
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// Gets or sets whether dense table layouts should be used.
    /// </summary>
    public bool CompactTables { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user navigation for this preference record.
    /// </summary>
    public AppUser User { get; set; }
}
