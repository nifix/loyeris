using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Query that lists authentication security events.
/// </summary>
public record GetAuthEventsQuery() : IRequest<Result<IReadOnlyList<AuthEventDto>>>;
