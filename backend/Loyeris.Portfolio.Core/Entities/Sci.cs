using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Portfolio.Core.Entities;

/// <summary>
/// Represents a French SCI structure that owns one or more rental lots.
/// </summary>
public class Sci
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the SCI.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workspace that owns this SCI.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Gets or sets the legal or display name of the SCI.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional French SIREN identifier.
    /// </summary>
    public string Siren { get; set; }

    /// <summary>
    /// Gets or sets the tax regime applied to the SCI.
    /// </summary>
    public TaxRegime TaxRegime { get; set; }

    /// <summary>
    /// Gets or sets whether the SCI is active or archived.
    /// </summary>
    public SciStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the street address of the SCI.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Gets or sets the postal code of the SCI address.
    /// </summary>
    public string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the city of the SCI address.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the ISO country code of the SCI address.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the legal incorporation date, when known.
    /// </summary>
    public DateOnly? IncorporatedOn { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the SCI was archived, if applicable.
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the associates linked to this SCI.
    /// </summary>
    public ICollection<SciAssociate> Associates { get; set; } = new List<SciAssociate>();

    /// <summary>
    /// Gets or sets the lots owned by this SCI.
    /// </summary>
    public ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
