namespace Loyeris.Portfolio.Core.Enums;

/// <summary>
/// Describes whether a rental unit is available in the managed portfolio.
/// </summary>
public enum LotStatus
{
    /// <summary>
    /// The lot is part of active portfolio operations.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The lot is retained for history but excluded from active workflows.
    /// </summary>
    Archived = 1
}
