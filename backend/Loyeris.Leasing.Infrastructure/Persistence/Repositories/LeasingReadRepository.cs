using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Leasing.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Leasing projections.
/// </summary>
public class LeasingReadRepository(LeasingDbContext dbContext) : ILeasingReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AvailableTenantDto>> ListAvailableTenantsAsync(
        Guid workspaceId,
        Guid? currentLotId,
        DateOnly currentDate,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tenants
            .AsNoTracking()
            .Where(tenant => tenant.WorkspaceId == workspaceId && tenant.Status == TenantStatus.Active)
            .Where(tenant => !tenant.LeaseTenants.Any(link =>
                (link.Lease.Status == LeaseStatus.Active || link.Lease.Status == LeaseStatus.Draft)
                && (link.Lease.EndsOn == null || link.Lease.EndsOn >= currentDate)
                && link.Lease.LotId != currentLotId))
            .OrderBy(tenant => tenant.LastName)
            .ThenBy(tenant => tenant.FirstName)
            .Select(tenant => new AvailableTenantDto(
                tenant.Id,
                tenant.FirstName,
                tenant.LastName,
                tenant.Email,
                currentLotId.HasValue && tenant.LeaseTenants.Any(link =>
                    link.Lease.LotId == currentLotId
                    && link.Lease.Status == LeaseStatus.Active
                    && (link.Lease.EndsOn == null || link.Lease.EndsOn >= currentDate))))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<LotOccupancyDto> GetLotOccupancyAsync(
        Guid workspaceId,
        Guid lotId,
        DateOnly currentDate,
        CancellationToken cancellationToken)
    {
        return dbContext.LeaseTenants
            .AsNoTracking()
            .Where(link => link.Tenant.WorkspaceId == workspaceId
                           && link.Lease.LotId == lotId
                           && link.Lease.Status == LeaseStatus.Active
                           && (link.Lease.EndsOn == null || link.Lease.EndsOn >= currentDate)
                           && link.Role == LeaseTenantRole.Primary)
            .Select(link => new LotOccupancyDto(
                link.Lease.LotId,
                link.LeaseId,
                link.TenantId,
                link.Tenant.FirstName,
                link.Tenant.LastName,
                link.Lease.StartsOn,
                link.Lease.EndsOn,
                link.Lease.RentDueDay,
                link.Lease.RentExcludingChargesCents,
                link.Lease.ChargesCents,
                link.Lease.DepositCents,
                link.Lease.PaymentTerms,
                link.Lease.Notes))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LotOccupancyDto>> ListLotOccupanciesAsync(
        Guid workspaceId,
        DateOnly periodStartsOn,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken)
    {
        return await dbContext.LeaseTenants
            .AsNoTracking()
            .Where(link => link.Tenant.WorkspaceId == workspaceId
                           && (link.Lease.Status == LeaseStatus.Active
                               || link.Lease.Status == LeaseStatus.Ended)
                           && link.Lease.StartsOn <= periodEndsOn
                           && (link.Lease.EndsOn == null || link.Lease.EndsOn >= periodStartsOn)
                           && link.Role == LeaseTenantRole.Primary)
            .OrderBy(link => link.Lease.StartsOn)
            .ThenBy(link => link.Tenant.LastName)
            .ThenBy(link => link.Tenant.FirstName)
            .Select(link => new LotOccupancyDto(
                link.Lease.LotId,
                link.LeaseId,
                link.TenantId,
                link.Tenant.FirstName,
                link.Tenant.LastName,
                link.Lease.StartsOn,
                link.Lease.EndsOn,
                link.Lease.RentDueDay,
                link.Lease.RentExcludingChargesCents,
                link.Lease.ChargesCents,
                link.Lease.DepositCents,
                link.Lease.PaymentTerms,
                link.Lease.Notes))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LotOccupancyDto>> ListLotLeaseHistoryAsync(
        Guid workspaceId,
        Guid lotId,
        CancellationToken cancellationToken)
    {
        return await dbContext.LeaseTenants
            .AsNoTracking()
            .Where(link => link.Tenant.WorkspaceId == workspaceId
                           && link.Lease.LotId == lotId
                           && (link.Lease.Status == LeaseStatus.Active
                               || link.Lease.Status == LeaseStatus.Ended)
                           && link.Role == LeaseTenantRole.Primary)
            .OrderByDescending(link => link.Lease.StartsOn)
            .Select(link => new LotOccupancyDto(
                link.Lease.LotId,
                link.LeaseId,
                link.TenantId,
                link.Tenant.FirstName,
                link.Tenant.LastName,
                link.Lease.StartsOn,
                link.Lease.EndsOn,
                link.Lease.RentDueDay,
                link.Lease.RentExcludingChargesCents,
                link.Lease.ChargesCents,
                link.Lease.DepositCents,
                link.Lease.PaymentTerms,
                link.Lease.Notes))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TenantDto>> ListTenantsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Tenants
            .AsNoTracking()
            .OrderBy(tenant => tenant.LastName)
            .ThenBy(tenant => tenant.FirstName)
            .Select(tenant => new TenantDto(
                tenant.Id,
                tenant.WorkspaceId,
                tenant.FirstName,
                tenant.LastName,
                tenant.Email,
                tenant.Phone,
                tenant.Status,
                tenant.CreatedAt,
                tenant.UpdatedAt,
                tenant.ArchivedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LeaseDto>> ListLeasesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Leases
            .AsNoTracking()
            .OrderByDescending(lease => lease.StartsOn)
            .Select(lease => new LeaseDto(
                lease.Id,
                lease.LotId,
                lease.Status,
                lease.StartsOn,
                lease.EndsOn,
                lease.RentDueDay,
                lease.RentExcludingChargesCents,
                lease.ChargesCents,
                lease.DepositCents,
                lease.PaymentTerms,
                lease.Notes,
                lease.CreatedAt,
                lease.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LeaseTenantDto>> ListLeaseTenantsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.LeaseTenants
            .AsNoTracking()
            .OrderBy(leaseTenant => leaseTenant.LeaseId)
            .ThenBy(leaseTenant => leaseTenant.Role)
            .Select(leaseTenant => new LeaseTenantDto(
                leaseTenant.Id,
                leaseTenant.LeaseId,
                leaseTenant.TenantId,
                leaseTenant.Role,
                leaseTenant.CreatedAt,
                leaseTenant.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
