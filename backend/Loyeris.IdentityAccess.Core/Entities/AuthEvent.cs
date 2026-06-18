using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Captures a lightweight security audit event for authentication workflows.
/// </summary>
public class AuthEvent
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the event.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user involved in the event, when known.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the session involved in the event, when known.
    /// </summary>
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the normalized email used for lookup when the user is unknown.
    /// </summary>
    public string NormalizedEmail { get; set; }

    /// <summary>
    /// Gets or sets the type of security event.
    /// </summary>
    public AuthEventType Type { get; set; }

    /// <summary>
    /// Gets or sets when the event occurred.
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address associated with the event.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent associated with the event.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the optional failure reason for rejected security actions.
    /// </summary>
    public string FailureReason { get; set; }

    /// <summary>
    /// Gets or sets optional structured metadata serialized as JSON.
    /// </summary>
    public string MetadataJson { get; set; }

    /// <summary>
    /// Gets or sets the user navigation for this event.
    /// </summary>
    public AppUser User { get; set; }

    /// <summary>
    /// Gets or sets the session navigation for this event.
    /// </summary>
    public AuthSession Session { get; set; }
}
