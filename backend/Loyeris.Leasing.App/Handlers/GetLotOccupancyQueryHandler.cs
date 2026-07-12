using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetLotOccupancyQuery"/>.
/// </summary>
public class GetLotOccupancyQueryHandler(
    ILeasingReadRepository repository,
    TimeProvider timeProvider)
    : IRequestHandler<GetLotOccupancyQuery, Result<LotOccupancyDto>>
{
    /// <inheritdoc />
    public async Task<Result<LotOccupancyDto>> Handle(
        GetLotOccupancyQuery request,
        CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var occupancy = await repository.GetLotOccupancyAsync(
            request.WorkspaceId,
            request.LotId,
            currentDate,
            cancellationToken);
        
        return Result<LotOccupancyDto>.Success(occupancy);
    }
}
