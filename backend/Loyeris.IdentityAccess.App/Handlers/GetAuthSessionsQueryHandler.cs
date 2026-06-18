using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Handles <see cref="GetAuthSessionsQuery"/>.
/// </summary>
public class GetAuthSessionsQueryHandler(IIdentityAccessReadRepository repository)
    : IRequestHandler<GetAuthSessionsQuery, Result<IReadOnlyList<AuthSessionDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AuthSessionDto>>> Handle(GetAuthSessionsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<AuthSessionDto>>.Success(await repository.ListAuthSessionsAsync(cancellationToken));
}
