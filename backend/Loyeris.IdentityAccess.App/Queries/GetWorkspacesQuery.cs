using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Query that lists workspaces.
/// </summary>
public record GetWorkspacesQuery() : IRequest<Result<IReadOnlyList<WorkspaceDto>>>;
