namespace Loyeris.Leasing.Core.Enums;

/// <summary>
/// Describes the lifecycle state of a rental lease.
/// </summary>
public enum LeaseStatus
{
    /// <summary>
    /// The lease is being prepared and is not active yet.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// The lease currently drives occupancy and rent generation.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The lease has ended and is kept for history.
    /// </summary>
    Ended = 2,

    /// <summary>
    /// The lease was canceled before or during its lifecycle.
    /// </summary>
    Canceled = 3
}
