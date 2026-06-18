using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Read model returned when listing authentication sessions.
/// </summary>
public record AuthSessionDto(
    Guid Id,
    Guid UserId,
    AuthSessionStatus Status,
    string DeviceLabel,
    string UserAgent,
    string IpAddress,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastSeenAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? RevokedAt,
    string RevokedReason);
