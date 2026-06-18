using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Handles <see cref="GetWorkspacesQuery"/>.
/// </summary>
public class GetWorkspacesQueryHandler(IIdentityAccessReadRepository repository)
    : IRequestHandler<GetWorkspacesQuery, Result<IReadOnlyList<WorkspaceDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<WorkspaceDto>>> Handle(GetWorkspacesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<WorkspaceDto>>.Success(await repository.ListWorkspacesAsync(cancellationToken));
}
