using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Read model returned when listing authentication security events.
/// </summary>
public record AuthEventDto(
    Guid Id,
    Guid? UserId,
    Guid? SessionId,
    string NormalizedEmail,
    AuthEventType Type,
    DateTimeOffset OccurredAt,
    string IpAddress,
    string UserAgent,
    string FailureReason);
