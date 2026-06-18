using Loyeris.RentCollection.Core.Enums;

namespace Loyeris.RentCollection.App.Dtos;

/// <summary>
/// Read model returned when listing rent deadlines.
/// </summary>
public record RentDeadlineDto(
    Guid Id,
    Guid LeaseId,
    DateOnly PeriodMonth,
    DateOnly DueOn,
    long RentExcludingChargesCents,
    long ChargesCents,
    long TotalDueCents,
    long PaidCents,
    long RemainingCents,
    RentDeadlineStatus Status,
    DateTimeOffset GeneratedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
