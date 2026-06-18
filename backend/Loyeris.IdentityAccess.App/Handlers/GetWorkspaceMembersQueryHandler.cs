using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Handles <see cref="GetWorkspaceMembersQuery"/>.
/// </summary>
public class GetWorkspaceMembersQueryHandler(IIdentityAccessReadRepository repository)
    : IRequestHandler<GetWorkspaceMembersQuery, Result<IReadOnlyList<WorkspaceMemberDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<WorkspaceMemberDto>>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<WorkspaceMemberDto>>.Success(await repository.ListWorkspaceMembersAsync(cancellationToken));
}
