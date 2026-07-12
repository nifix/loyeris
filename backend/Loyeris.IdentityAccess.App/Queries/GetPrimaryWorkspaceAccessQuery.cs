using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Resolves the active workspace currently used for an authenticated user.
/// </summary>
public record GetPrimaryWorkspaceAccessQuery(Guid UserId) : IRequest<Result<WorkspaceAccessDto>>;
