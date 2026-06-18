namespace Loyeris.Messaging.Core.Enums;

/// <summary>
/// Describes the processing state of an outbox message.
/// </summary>
public enum OutboxMessageStatus
{
    /// <summary>
    /// The message is waiting to be processed.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The message is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The message has been processed successfully.
    /// </summary>
    Processed = 2,

    /// <summary>
    /// Processing failed and the error should be inspected or retried.
    /// </summary>
    Failed = 3
}
