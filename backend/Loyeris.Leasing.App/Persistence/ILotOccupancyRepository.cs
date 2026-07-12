using Loyeris.Leasing.Core.Entities;

namespace Loyeris.Leasing.App.Persistence;

/// <summary>
/// Provides persistence for active lot occupancies.
/// </summary>
public interface ILotOccupancyRepository
{
    /// <summary>
    /// Gets the active lease for a lot, including its tenant links.
    /// </summary>
    Task<Lease> GetActiveLeaseAsync(Guid lotId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets one editable lease and verifies that it belongs to the workspace and lot.
    /// </summary>
    Task<Lease> GetLeaseAsync(
        Guid workspaceId,
        Guid lotId,
        Guid leaseId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether another lease overlaps the requested dates on the same lot.
    /// </summary>
    Task<bool> HasOverlappingLeaseAsync(
        Guid lotId,
        Guid leaseId,
        DateOnly startsOn,
        DateOnly? endsOn,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets an active tenant inside the workspace.
    /// </summary>
    Task<Tenant> GetActiveTenantAsync(Guid workspaceId, Guid tenantId, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a tenant is already assigned to another current lease.
    /// </summary>
    Task<bool> IsAssignedElsewhereAsync(
        Guid tenantId,
        Guid lotId,
        DateOnly currentDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new lease aggregate.
    /// </summary>
    Task AddAsync(Lease lease, CancellationToken cancellationToken);

    /// <summary>
    /// Saves occupancy changes atomically inside the Leasing context.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
