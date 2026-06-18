using Loyeris.TaxPreparation.Core.Enums;

namespace Loyeris.TaxPreparation.App.Dtos;

/// <summary>
/// Read model returned when listing fiscal periods.
/// </summary>
public record FiscalPeriodDto(
    Guid Id,
    Guid SciId,
    int Year,
    DateOnly StartsOn,
    DateOnly EndsOn,
    FiscalPeriodStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
