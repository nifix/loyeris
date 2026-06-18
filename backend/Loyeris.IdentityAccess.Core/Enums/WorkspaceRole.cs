namespace Loyeris.IdentityAccess.Core.Enums;

/// <summary>
/// Defines the permission level granted to a user within a workspace.
/// </summary>
public enum WorkspaceRole
{
    /// <summary>
    /// Full owner role with administrative control over the workspace.
    /// </summary>
    Owner = 0,

    /// <summary>
    /// Administrative role without ownership semantics.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Standard member role for day-to-day application usage.
    /// </summary>
    Member = 2
}
