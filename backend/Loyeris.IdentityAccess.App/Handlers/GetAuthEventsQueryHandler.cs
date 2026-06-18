using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Handles <see cref="GetAuthEventsQuery"/>.
/// </summary>
public class GetAuthEventsQueryHandler(IIdentityAccessReadRepository repository)
    : IRequestHandler<GetAuthEventsQuery, Result<IReadOnlyList<AuthEventDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AuthEventDto>>> Handle(GetAuthEventsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<AuthEventDto>>.Success(await repository.ListAuthEventsAsync(cancellationToken));
}
