namespace Loyeris.TaxPreparation.Core.Enums;

/// <summary>
/// Describes whether a fiscal period is still editable or closed.
/// </summary>
public enum FiscalPeriodStatus
{
    /// <summary>
    /// The fiscal period is open for preparation and changes.
    /// </summary>
    Open = 0,

    /// <summary>
    /// The fiscal period has been closed for reporting.
    /// </summary>
    Closed = 1
}
