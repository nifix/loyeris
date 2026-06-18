using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Query that lists application users.
/// </summary>
public record GetUsersQuery() : IRequest<Result<IReadOnlyList<UserDto>>>;
