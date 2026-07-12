using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetLotOccupanciesQuery"/>.
/// </summary>
public class GetLotOccupanciesQueryHandler(
    ILeasingReadRepository repository,
    TimeProvider timeProvider)
    : IRequestHandler<GetLotOccupanciesQuery, Result<IReadOnlyList<LotOccupancyDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<LotOccupancyDto>>> Handle(
        GetLotOccupanciesQuery request,
        CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var monthStartsOn = new DateOnly(currentDate.Year, currentDate.Month, 1);
        var monthEndsOn = monthStartsOn.AddMonths(1).AddDays(-1);
        
        return Result<IReadOnlyList<LotOccupancyDto>>.Success(
            await repository.ListLotOccupanciesAsync(
                request.WorkspaceId,
                monthStartsOn,
                monthEndsOn,
                cancellationToken));
    }
}
