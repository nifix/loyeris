using Loyeris.Portfolio.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Queries;

/// <summary>
/// Query that returns one rental lot from a workspace.
/// </summary>
public record GetLotQuery(Guid WorkspaceId, Guid LotId) : IRequest<Result<LotDto>>;
