namespace Loyeris.TaxPreparation.App.Dtos;

/// <summary>
/// Read model returned when listing rental expenses.
/// </summary>
public record RentalExpenseDto(
    Guid Id,
    Guid SciId,
    Guid? LotId,
    DateOnly ExpenseDate,
    string Category,
    string Label,
    long AmountCents,
    bool DeductibleForIr,
    string DocumentUrl,
    string Note,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
