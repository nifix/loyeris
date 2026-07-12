using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Commands;

/// <summary>
/// Removes a lease from active use while preserving its persisted audit history.
/// </summary>
public record DeleteLotLeaseCommand(
    Guid WorkspaceId,
    Guid LotId,
    Guid LeaseId) : IRequest<Result<LotOccupancyDto>>;
