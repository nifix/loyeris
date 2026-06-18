using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Read model returned when listing workspaces.
/// </summary>
public record WorkspaceDto(
    Guid Id,
    string Name,
    Guid OwnerUserId,
    WorkspaceStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ArchivedAt);
