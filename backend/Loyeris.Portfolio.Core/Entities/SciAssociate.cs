namespace Loyeris.Portfolio.Core.Entities;

/// <summary>
/// Represents an associate owning shares in an SCI.
/// </summary>
public class SciAssociate
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the associate record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SCI this associate belongs to.
    /// </summary>
    public Guid SciId { get; set; }

    /// <summary>
    /// Gets or sets the associate's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the associate's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the optional contact email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the number of shares owned by the associate.
    /// </summary>
    public int? SharesCount { get; set; }

    /// <summary>
    /// Gets or sets the ownership percentage when shares are expressed as a ratio.
    /// </summary>
    public decimal? OwnershipPercentage { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the SCI navigation.
    /// </summary>
    public Sci Sci { get; set; }
}
