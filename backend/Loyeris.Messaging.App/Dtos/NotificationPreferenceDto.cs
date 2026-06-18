namespace Loyeris.Messaging.App.Dtos;

/// <summary>
/// Read model returned when listing notification preferences.
/// </summary>
public record NotificationPreferenceDto(
    Guid Id,
    Guid UserId,
    bool RentOverdueEmailEnabled,
    bool PaymentRecordedEmailEnabled,
    bool LeaseEndingEmailEnabled,
    int LeaseEndingNoticeMonths,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
