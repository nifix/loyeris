using Loyeris.RentCollection.Core.Enums;

namespace Loyeris.RentCollection.App.Dtos;

/// <summary>
/// Read model returned when listing rent reminders.
/// </summary>
public record RentReminderDto(
    Guid Id,
    Guid RentDeadlineId,
    RentReminderChannel Channel,
    RentReminderStatus Status,
    DateTimeOffset? ScheduledFor,
    DateTimeOffset? SentAt,
    string RecipientEmail,
    string Subject,
    string Body,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
