using Loyeris.RentCollection.Core.Enums;

namespace Loyeris.RentCollection.App.Dtos;

/// <summary>
/// Read model returned when listing rent payments.
/// </summary>
public record RentPaymentDto(
    Guid Id,
    Guid RentDeadlineId,
    long AmountCents,
    DateOnly PaidOn,
    Guid RecordedByUserId,
    PaymentMethod Method,
    string Reference,
    string Note,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
