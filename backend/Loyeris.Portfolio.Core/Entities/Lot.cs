using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Portfolio.Core.Entities;

/// <summary>
/// Represents a rentable unit owned by an SCI, such as an apartment, garage, or commercial unit.
/// </summary>
public class Lot
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the lot.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SCI that owns the lot.
    /// </summary>
    public Guid SciId { get; set; }

    /// <summary>
    /// Gets or sets the human-readable lot reference, for example "Lot A02".
    /// </summary>
    public string Reference { get; set; }

    /// <summary>
    /// Gets or sets the lot classification.
    /// </summary>
    public LotType Type { get; set; }

    /// <summary>
    /// Gets or sets whether the lot is active or archived.
    /// </summary>
    public LotStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the street address of the lot.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Gets or sets the postal code of the lot address.
    /// </summary>
    public string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the city of the lot address.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the ISO country code of the lot address.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the lot surface in square meters.
    /// </summary>
    public decimal? SurfaceSqm { get; set; }

    /// <summary>
    /// Gets or sets the expected rent excluding charges, stored in cents.
    /// </summary>
    public long PotentialRentExcludingChargesCents { get; set; }

    /// <summary>
    /// Gets or sets the expected monthly charges, stored in cents.
    /// </summary>
    public long PotentialChargesCents { get; set; }

    /// <summary>
    /// Gets or sets the suggested security deposit, stored in cents.
    /// </summary>
    public long SuggestedDepositCents { get; set; }

    /// <summary>
    /// Gets or sets internal notes about the lot.
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the lot was archived, if applicable.
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the owning SCI navigation.
    /// </summary>
    public Sci Sci { get; set; }
}
