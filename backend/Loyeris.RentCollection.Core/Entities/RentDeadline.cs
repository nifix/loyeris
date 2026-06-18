using Loyeris.RentCollection.Core.Enums;

namespace Loyeris.RentCollection.Core.Entities;

/// <summary>
/// Represents a generated monthly rent amount expected for a lease.
/// </summary>
public class RentDeadline
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the rent deadline.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the lease identifier that produced this deadline.
    /// </summary>
    public Guid LeaseId { get; set; }

    /// <summary>
    /// Gets or sets the represented month, stored as the first day of that month.
    /// </summary>
    public DateOnly PeriodMonth { get; set; }

    /// <summary>
    /// Gets or sets the due date for the expected rent.
    /// </summary>
    public DateOnly DueOn { get; set; }

    /// <summary>
    /// Gets or sets the snapshotted rent excluding charges, stored in cents.
    /// </summary>
    public long RentExcludingChargesCents { get; set; }

    /// <summary>
    /// Gets or sets the snapshotted charges, stored in cents.
    /// </summary>
    public long ChargesCents { get; set; }

    /// <summary>
    /// Gets or sets the total expected amount, stored in cents.
    /// </summary>
    public long TotalDueCents { get; set; }

    /// <summary>
    /// Gets or sets the total amount already paid, stored in cents.
    /// </summary>
    public long PaidCents { get; set; }

    /// <summary>
    /// Gets or sets the amount still expected, stored in cents.
    /// </summary>
    public long RemainingCents { get; set; }

    /// <summary>
    /// Gets or sets the settlement status of the deadline.
    /// </summary>
    public RentDeadlineStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when this monthly deadline was generated.
    /// </summary>
    public DateTimeOffset GeneratedAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the payments recorded against this deadline.
    /// </summary>
    public ICollection<RentPayment> Payments { get; set; } = new List<RentPayment>();

    /// <summary>
    /// Gets or sets the reminders linked to this deadline.
    /// </summary>
    public ICollection<RentReminder> Reminders { get; set; } = new List<RentReminder>();
}
