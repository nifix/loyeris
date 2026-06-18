namespace Loyeris.RentCollection.Core.Enums;

/// <summary>
/// Describes the lifecycle state of a rent reminder.
/// </summary>
public enum RentReminderStatus
{
    /// <summary>
    /// The reminder is planned but not sent yet.
    /// </summary>
    Scheduled = 0,

    /// <summary>
    /// The reminder has been sent or completed.
    /// </summary>
    Sent = 1,

    /// <summary>
    /// The reminder could not be completed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// The reminder was canceled before execution.
    /// </summary>
    Canceled = 3
}
