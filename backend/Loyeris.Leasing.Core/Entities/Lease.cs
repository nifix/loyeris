using Loyeris.Leasing.Core.Enums;

namespace Loyeris.Leasing.Core.Entities;

/// <summary>
/// Represents a rental contract for a lot and acts as the source of truth for occupancy.
/// </summary>
public class Lease
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the lease.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the portfolio lot identifier. The lot itself belongs to the Portfolio domain.
    /// </summary>
    public Guid LotId { get; set; }

    /// <summary>
    /// Gets or sets the lifecycle status of the lease.
    /// </summary>
    public LeaseStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the lease start date.
    /// </summary>
    public DateOnly StartsOn { get; set; }

    /// <summary>
    /// Gets or sets the lease end date, when already known.
    /// </summary>
    public DateOnly? EndsOn { get; set; }

    /// <summary>
    /// Gets or sets the day of month on which rent is due.
    /// </summary>
    public int RentDueDay { get; set; }

    /// <summary>
    /// Gets or sets the monthly rent excluding charges, stored in cents.
    /// </summary>
    public long RentExcludingChargesCents { get; set; }

    /// <summary>
    /// Gets or sets the monthly charges, stored in cents.
    /// </summary>
    public long ChargesCents { get; set; }

    /// <summary>
    /// Gets or sets the security deposit, stored in cents.
    /// </summary>
    public long DepositCents { get; set; }

    /// <summary>
    /// Gets or sets free-form payment terms attached to the lease.
    /// </summary>
    public string PaymentTerms { get; set; }

    /// <summary>
    /// Gets or sets internal notes about the lease.
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
    /// Gets or sets the tenants and guarantors linked to this lease.
    /// </summary>
    public ICollection<LeaseTenant> LeaseTenants { get; set; } = new List<LeaseTenant>();
}
