using Loyeris.Leasing.Core.Enums;

namespace Loyeris.Leasing.App.Dtos;

/// <summary>
/// Read model returned when listing leases.
/// </summary>
public record LeaseDto(
    Guid Id,
    Guid LotId,
    LeaseStatus Status,
    DateOnly StartsOn,
    DateOnly? EndsOn,
    int RentDueDay,
    long RentExcludingChargesCents,
    long ChargesCents,
    long DepositCents,
    string PaymentTerms,
    string Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
