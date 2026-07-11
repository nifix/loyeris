using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Identity Access projections.
/// </summary>
public class IdentityAccessReadRepository(IdentityAccessDbContext dbContext) : IIdentityAccessReadRepository
{
    /// <inheritdoc />
    public Task<WorkspaceAccessDto> GetPrimaryWorkspaceAccessAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.WorkspaceMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId && member.Workspace.Status == WorkspaceStatus.Active)
            .OrderBy(member => member.Role == WorkspaceRole.Owner ? 0 : member.Role == WorkspaceRole.Admin ? 1 : 2)
            .ThenBy(member => member.JoinedAt)
            .Select(member => new WorkspaceAccessDto(member.WorkspaceId, member.Role))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.AppUsers
            .AsNoTracking()
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .ThenBy(user => user.Email)
            .Select(user => new UserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Phone,
                user.Status,
                user.EmailConfirmedAt,
                user.LastLoginAt,
                user.CreatedAt,
                user.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkspaceDto>> ListWorkspacesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Workspaces
            .AsNoTracking()
            .OrderBy(workspace => workspace.Name)
            .Select(workspace => new WorkspaceDto(
                workspace.Id,
                workspace.Name,
                workspace.OwnerUserId,
                workspace.Status,
                workspace.CreatedAt,
                workspace.UpdatedAt,
                workspace.ArchivedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkspaceMemberDto>> ListWorkspaceMembersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceMembers
            .AsNoTracking()
            .OrderByDescending(member => member.JoinedAt)
            .Select(member => new WorkspaceMemberDto(
                member.Id,
                member.WorkspaceId,
                member.UserId,
                member.Role,
                member.JoinedAt,
                member.CreatedAt,
                member.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuthSessionDto>> ListAuthSessionsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.AuthSessions
            .AsNoTracking()
            .OrderByDescending(session => session.CreatedAt)
            .Select(session => new AuthSessionDto(
                session.Id,
                session.UserId,
                session.Status,
                session.DeviceLabel,
                session.UserAgent,
                session.IpAddress,
                session.CreatedAt,
                session.LastSeenAt,
                session.ExpiresAt,
                session.RevokedAt,
                session.RevokedReason))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuthEventDto>> ListAuthEventsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.AuthEvents
            .AsNoTracking()
            .OrderByDescending(authEvent => authEvent.OccurredAt)
            .Select(authEvent => new AuthEventDto(
                authEvent.Id,
                authEvent.UserId,
                authEvent.SessionId,
                authEvent.NormalizedEmail,
                authEvent.Type,
                authEvent.OccurredAt,
                authEvent.IpAddress,
                authEvent.UserAgent,
                authEvent.FailureReason))
            .ToListAsync(cancellationToken);
    }
}
