using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Resolves the first active workspace available to the current user.
/// </summary>
public class GetPrimaryWorkspaceAccessQueryHandler(IIdentityAccessReadRepository repository)
    : IRequestHandler<GetPrimaryWorkspaceAccessQuery, Result<WorkspaceAccessDto>>
{
    private static readonly Error WorkspaceUnavailable = new(
        "identity.workspace_unavailable",
        "Aucun espace de travail actif n'est disponible.",
        ErrorType.Forbidden);

    /// <inheritdoc />
    public async Task<Result<WorkspaceAccessDto>> Handle(
        GetPrimaryWorkspaceAccessQuery request,
        CancellationToken cancellationToken)
    {
        var access = await repository.GetPrimaryWorkspaceAccessAsync(request.UserId, cancellationToken);
        return access is null
            ? Result<WorkspaceAccessDto>.Failure(WorkspaceUnavailable)
            : Result<WorkspaceAccessDto>.Success(access);
    }
}
