namespace Loyeris.Messaging.Core.Entities;

/// <summary>
/// Stores notification settings for a user.
/// </summary>
public class NotificationPreference
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the preference row.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user these notification settings belong to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets whether overdue rent emails are enabled.
    /// </summary>
    public bool RentOverdueEmailEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether payment recorded emails are enabled.
    /// </summary>
    public bool PaymentRecordedEmailEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether lease ending emails are enabled.
    /// </summary>
    public bool LeaseEndingEmailEnabled { get; set; }

    /// <summary>
    /// Gets or sets how many months before the lease end date the alert should be sent.
    /// </summary>
    public int LeaseEndingNoticeMonths { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
