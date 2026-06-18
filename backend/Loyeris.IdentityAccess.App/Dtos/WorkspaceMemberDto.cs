using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Read model returned when listing workspace memberships.
/// </summary>
public record WorkspaceMemberDto(
    Guid Id,
    Guid WorkspaceId,
    Guid UserId,
    WorkspaceRole Role,
    DateTimeOffset JoinedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
