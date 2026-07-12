using Loyeris.Leasing.App.Dtos;

namespace Loyeris.Leasing.App.Persistence;

/// <summary>
/// Provides read-only projections for Leasing screens and endpoints.
/// </summary>
public interface ILeasingReadRepository
{
    /// <summary>
    /// Lists active tenants that are not assigned to another active or draft lease.
    /// </summary>
    Task<IReadOnlyList<AvailableTenantDto>> ListAvailableTenantsAsync(
        Guid workspaceId,
        Guid? currentLotId,
        DateOnly currentDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the active primary occupancy for one lot.
    /// </summary>
    Task<LotOccupancyDto> GetLotOccupancyAsync(
        Guid workspaceId,
        Guid lotId,
        DateOnly currentDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists primary occupancies whose leases overlap the requested period.
    /// </summary>
    Task<IReadOnlyList<LotOccupancyDto>> ListLotOccupanciesAsync(
        Guid workspaceId,
        DateOnly periodStartsOn,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists the recent primary leases attached to one lot.
    /// </summary>
    Task<IReadOnlyList<LotOccupancyDto>> ListLotLeaseHistoryAsync(
        Guid workspaceId,
        Guid lotId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists tenants.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The tenants.</returns>
    Task<IReadOnlyList<TenantDto>> ListTenantsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists leases.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The leases.</returns>
    Task<IReadOnlyList<LeaseDto>> ListLeasesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists lease tenant links.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The lease tenant links.</returns>
    Task<IReadOnlyList<LeaseTenantDto>> ListLeaseTenantsAsync(CancellationToken cancellationToken);
}
