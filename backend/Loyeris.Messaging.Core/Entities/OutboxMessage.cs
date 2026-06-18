using Loyeris.Messaging.Core.Enums;

namespace Loyeris.Messaging.Core.Entities;

/// <summary>
/// Represents a reliable asynchronous message waiting to be handled by a background worker.
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the outbox message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the message type used by handlers to route processing.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the JSON payload serialized for the message handler.
    /// </summary>
    public string Payload { get; set; }

    /// <summary>
    /// Gets or sets the processing status of the outbox message.
    /// </summary>
    public OutboxMessageStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the earliest timestamp at which the message can be processed.
    /// </summary>
    public DateTimeOffset AvailableAt { get; set; }

    /// <summary>
    /// Gets or sets when the message was processed, if applicable.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>
    /// Gets or sets the last processing error, if any.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
