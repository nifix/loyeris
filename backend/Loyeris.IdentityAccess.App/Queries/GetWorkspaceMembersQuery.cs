using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Query that lists workspace memberships.
/// </summary>
public record GetWorkspaceMembersQuery() : IRequest<Result<IReadOnlyList<WorkspaceMemberDto>>>;
