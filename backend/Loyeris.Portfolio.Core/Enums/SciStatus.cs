namespace Loyeris.Portfolio.Core.Enums;

/// <summary>
/// Describes whether an SCI is actively managed or archived.
/// </summary>
public enum SciStatus
{
    /// <summary>
    /// The SCI is active in the current portfolio.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The SCI is kept for historical records only.
    /// </summary>
    Archived = 1
}
