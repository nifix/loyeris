using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetLotLeaseHistoryQuery"/>.
/// </summary>
public class GetLotLeaseHistoryQueryHandler(ILeasingReadRepository repository)
    : IRequestHandler<GetLotLeaseHistoryQuery, Result<IReadOnlyList<LotOccupancyDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<LotOccupancyDto>>> Handle(
        GetLotLeaseHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var leases = await repository.ListLotLeaseHistoryAsync(
            request.WorkspaceId,
            request.LotId,
            cancellationToken);
        return Result<IReadOnlyList<LotOccupancyDto>>.Success(leases);
    }
}
