using Loyeris.Leasing.App.Dtos;

namespace Loyeris.Leasing.App.Persistence;

/// <summary>
/// Provides read-only projections for Leasing screens and endpoints.
/// </summary>
public interface ILeasingReadRepository
{
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
