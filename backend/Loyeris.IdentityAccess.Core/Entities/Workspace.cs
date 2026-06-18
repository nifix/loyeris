using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Represents a data container for one rental management portfolio.
/// </summary>
public class Workspace
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the workspace.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the workspace.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the owner user identifier.
    /// </summary>
    public Guid OwnerUserId { get; set; }

    /// <summary>
    /// Gets or sets whether the workspace is active or archived.
    /// </summary>
    public WorkspaceStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the workspace was archived, if applicable.
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the owner user navigation.
    /// </summary>
    public AppUser OwnerUser { get; set; }

    /// <summary>
    /// Gets or sets the users granted access to the workspace.
    /// </summary>
    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
}
