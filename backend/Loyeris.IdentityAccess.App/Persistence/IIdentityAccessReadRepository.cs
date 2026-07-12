using Loyeris.IdentityAccess.App.Dtos;

namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Provides read-only projections for Identity Access screens and endpoints.
/// </summary>
public interface IIdentityAccessReadRepository
{
    /// <summary>
    /// Resolves the primary active workspace available to a user.
    /// </summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The workspace access, or null when the user has no active workspace.</returns>
    Task<WorkspaceAccessDto> GetPrimaryWorkspaceAccessAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists application users without sensitive authentication data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The application users.</returns>
    Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists workspaces.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The workspaces.</returns>
    Task<IReadOnlyList<WorkspaceDto>> ListWorkspacesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists workspace memberships.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The workspace memberships.</returns>
    Task<IReadOnlyList<WorkspaceMemberDto>> ListWorkspaceMembersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists authentication sessions.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The authentication sessions.</returns>
    Task<IReadOnlyList<AuthSessionDto>> ListAuthSessionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists authentication security events.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The authentication security events.</returns>
    Task<IReadOnlyList<AuthEventDto>> ListAuthEventsAsync(CancellationToken cancellationToken);
}
