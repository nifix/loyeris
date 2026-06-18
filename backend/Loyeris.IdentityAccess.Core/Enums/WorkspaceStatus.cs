namespace Loyeris.IdentityAccess.Core.Enums;

/// <summary>
/// Describes whether a workspace is currently usable or archived.
/// </summary>
public enum WorkspaceStatus
{
    /// <summary>
    /// The workspace is available for normal use.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The workspace is retained for history but hidden from regular operations.
    /// </summary>
    Archived = 1
}
