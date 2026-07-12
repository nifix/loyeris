using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that lists recent leases for one lot inside a workspace.
/// </summary>
public record GetLotLeaseHistoryQuery(
    Guid WorkspaceId,
    Guid LotId) : IRequest<Result<IReadOnlyList<LotOccupancyDto>>>;
