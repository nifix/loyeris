using Loyeris.TaxPreparation.Core.Enums;

namespace Loyeris.TaxPreparation.Core.Entities;

/// <summary>
/// Represents the fiscal year window prepared for an SCI.
/// </summary>
public class FiscalPeriod
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the fiscal period.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SCI identifier this fiscal period belongs to.
    /// </summary>
    public Guid SciId { get; set; }

    /// <summary>
    /// Gets or sets the fiscal year.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the first day of the fiscal period.
    /// </summary>
    public DateOnly StartsOn { get; set; }

    /// <summary>
    /// Gets or sets the last day of the fiscal period.
    /// </summary>
    public DateOnly EndsOn { get; set; }

    /// <summary>
    /// Gets or sets whether the period is open or closed.
    /// </summary>
    public FiscalPeriodStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
