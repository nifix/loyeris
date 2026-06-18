namespace Loyeris.RentCollection.Core.Enums;

/// <summary>
/// Identifies the channel used to perform a rent reminder.
/// </summary>
public enum RentReminderChannel
{
    /// <summary>
    /// Reminder sent by email.
    /// </summary>
    Email = 0,

    /// <summary>
    /// Manual reminder action tracked by the user.
    /// </summary>
    Manual = 1
}
