using Loyeris.RentCollection.Core.Enums;

namespace Loyeris.RentCollection.Core.Entities;

/// <summary>
/// Represents a reminder action attached to an unpaid or partially paid rent deadline.
/// </summary>
public class RentReminder
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the reminder.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the rent deadline that needs follow-up.
    /// </summary>
    public Guid RentDeadlineId { get; set; }

    /// <summary>
    /// Gets or sets the communication channel used for the reminder.
    /// </summary>
    public RentReminderChannel Channel { get; set; }

    /// <summary>
    /// Gets or sets the delivery or completion status of the reminder.
    /// </summary>
    public RentReminderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when the reminder is planned.
    /// </summary>
    public DateTimeOffset? ScheduledFor { get; set; }

    /// <summary>
    /// Gets or sets when the reminder was actually sent.
    /// </summary>
    public DateTimeOffset? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the recipient email address, when the reminder is sent by email.
    /// </summary>
    public string RecipientEmail { get; set; }

    /// <summary>
    /// Gets or sets the reminder subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the reminder body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the rent deadline navigation.
    /// </summary>
    public RentDeadline RentDeadline { get; set; }
}
