using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that returns the active primary occupancy for a lot.
/// </summary>
public record GetLotOccupancyQuery(
    Guid WorkspaceId,
    Guid LotId) : IRequest<Result<LotOccupancyDto>>;
