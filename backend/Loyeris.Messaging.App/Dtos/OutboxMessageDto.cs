using Loyeris.Messaging.Core.Enums;

namespace Loyeris.Messaging.App.Dtos;

/// <summary>
/// Read model returned when listing outbox messages without message payload content.
/// </summary>
public record OutboxMessageDto(
    Guid Id,
    string Type,
    OutboxMessageStatus Status,
    DateTimeOffset AvailableAt,
    DateTimeOffset? ProcessedAt,
    string Error,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
