using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetAvailableTenantsQuery"/>.
/// </summary>
public class GetAvailableTenantsQueryHandler(
    ILeasingReadRepository repository,
    TimeProvider timeProvider)
    : IRequestHandler<GetAvailableTenantsQuery, Result<IReadOnlyList<AvailableTenantDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AvailableTenantDto>>> Handle(
        GetAvailableTenantsQuery request,
        CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        return Result<IReadOnlyList<AvailableTenantDto>>.Success(
            await repository.ListAvailableTenantsAsync(
                request.WorkspaceId,
                request.CurrentLotId,
                currentDate,
                cancellationToken));
    }
}
