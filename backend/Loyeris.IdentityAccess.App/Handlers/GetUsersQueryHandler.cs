using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Handlers;

/// <summary>
/// Handles <see cref="GetUsersQuery"/>.
/// </summary>
public class GetUsersQueryHandler(IIdentityAccessReadRepository repository)
    : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<UserDto>>.Success(await repository.ListUsersAsync(cancellationToken));
}
