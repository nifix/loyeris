using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Grants a user access to a workspace with a specific role.
/// </summary>
public class WorkspaceMember
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the membership.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workspace identifier.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Gets or sets the member user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the role granted to the user inside the workspace.
    /// </summary>
    public WorkspaceRole Role { get; set; }

    /// <summary>
    /// Gets or sets when the user joined the workspace.
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the workspace navigation.
    /// </summary>
    public Workspace Workspace { get; set; }

    /// <summary>
    /// Gets or sets the user navigation.
    /// </summary>
    public AppUser User { get; set; }
}
