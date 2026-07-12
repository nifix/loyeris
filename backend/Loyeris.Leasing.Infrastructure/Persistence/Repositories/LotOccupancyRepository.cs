using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Leasing.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core persistence for active lot occupancies.
/// </summary>
public class LotOccupancyRepository(LeasingDbContext dbContext) : ILotOccupancyRepository
{
    /// <inheritdoc />
    public Task<Lease> GetActiveLeaseAsync(Guid lotId, CancellationToken cancellationToken)
    {
        return dbContext.Leases
            .Include(lease => lease.LeaseTenants)
            .ThenInclude(link => link.Tenant)
            .SingleOrDefaultAsync(
                lease => lease.LotId == lotId && lease.Status == LeaseStatus.Active,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<Lease> GetLeaseAsync(
        Guid workspaceId,
        Guid lotId,
        Guid leaseId,
        CancellationToken cancellationToken)
    {
        return dbContext.Leases
            .Include(lease => lease.LeaseTenants)
            .ThenInclude(link => link.Tenant)
            .SingleOrDefaultAsync(
                lease => lease.Id == leaseId
                         && lease.LotId == lotId
                         && lease.LeaseTenants.Any(link =>
                             link.Role == LeaseTenantRole.Primary
                             && link.Tenant.WorkspaceId == workspaceId),
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> HasOverlappingLeaseAsync(
        Guid lotId,
        Guid leaseId,
        DateOnly startsOn,
        DateOnly? endsOn,
        CancellationToken cancellationToken)
    {
        var latestDate = endsOn ?? DateOnly.MaxValue;
        return dbContext.Leases.AnyAsync(
            lease => lease.LotId == lotId
                     && lease.Id != leaseId
                     && (lease.Status == LeaseStatus.Active || lease.Status == LeaseStatus.Ended)
                     && lease.StartsOn <= latestDate
                     && (lease.EndsOn == null || lease.EndsOn >= startsOn),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<Tenant> GetActiveTenantAsync(
        Guid workspaceId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return dbContext.Tenants.SingleOrDefaultAsync(
            tenant => tenant.Id == tenantId
                      && tenant.WorkspaceId == workspaceId
                      && tenant.Status == TenantStatus.Active,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> IsAssignedElsewhereAsync(
        Guid tenantId,
        Guid lotId,
        DateOnly currentDate,
        CancellationToken cancellationToken)
    {
        return dbContext.LeaseTenants.AnyAsync(
            link => link.TenantId == tenantId
                    && link.Lease.LotId != lotId
                    && (link.Lease.Status == LeaseStatus.Active || link.Lease.Status == LeaseStatus.Draft)
                    && (link.Lease.EndsOn == null || link.Lease.EndsOn >= currentDate),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Lease lease, CancellationToken cancellationToken)
    {
        await dbContext.Leases.AddAsync(lease, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
