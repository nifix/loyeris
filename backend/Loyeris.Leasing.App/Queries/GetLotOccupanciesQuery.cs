using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that lists primary occupancies overlapping the current month for a workspace.
/// </summary>
public record GetLotOccupanciesQuery(Guid WorkspaceId) : IRequest<Result<IReadOnlyList<LotOccupancyDto>>>;
