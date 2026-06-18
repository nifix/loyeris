namespace Loyeris.RentCollection.Core.Enums;

/// <summary>
/// Describes the settlement state of a monthly rent deadline.
/// </summary>
public enum RentDeadlineStatus
{
    /// <summary>
    /// No complete payment has been recorded yet.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// One or more payments were recorded, but money is still due.
    /// </summary>
    Partial = 1,

    /// <summary>
    /// The expected amount has been fully paid.
    /// </summary>
    Paid = 2,

    /// <summary>
    /// The due date has passed and an amount remains unpaid.
    /// </summary>
    Overdue = 3,

    /// <summary>
    /// The deadline was canceled and should no longer be collected.
    /// </summary>
    Canceled = 4
}
