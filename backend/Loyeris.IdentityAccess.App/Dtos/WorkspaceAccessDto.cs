using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Identifies an active workspace available to a user and the role granted inside it.
/// </summary>
public record WorkspaceAccessDto(Guid WorkspaceId, WorkspaceRole Role);
