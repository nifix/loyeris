namespace Loyeris.TaxPreparation.Core.Entities;

/// <summary>
/// Represents an expense recorded for an SCI, optionally attached to a specific lot.
/// </summary>
public class RentalExpense
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the expense.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SCI that owns this expense.
    /// </summary>
    public Guid SciId { get; set; }

    /// <summary>
    /// Gets or sets the lot identifier when the expense is attributable to one lot.
    /// </summary>
    public Guid? LotId { get; set; }

    /// <summary>
    /// Gets or sets the date on which the expense occurred.
    /// </summary>
    public DateOnly ExpenseDate { get; set; }

    /// <summary>
    /// Gets or sets the fiscal or operational expense category.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the human-readable expense label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the expense amount, stored in cents.
    /// </summary>
    public long AmountCents { get; set; }

    /// <summary>
    /// Gets or sets whether the expense is considered deductible for SCI income tax preparation.
    /// </summary>
    public bool DeductibleForIr { get; set; }

    /// <summary>
    /// Gets or sets the optional document URL for supporting evidence.
    /// </summary>
    public string DocumentUrl { get; set; }

    /// <summary>
    /// Gets or sets optional internal notes about the expense.
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
