using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Query that lists authentication sessions.
/// </summary>
public record GetAuthSessionsQuery() : IRequest<Result<IReadOnlyList<AuthSessionDto>>>;
