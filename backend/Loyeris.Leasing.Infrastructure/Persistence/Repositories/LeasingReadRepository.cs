using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Leasing.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Leasing projections.
/// </summary>
public class LeasingReadRepository(LeasingDbContext dbContext) : ILeasingReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<TenantDto>> ListTenantsAsync(CancellationToken cancellationToken)
        => await dbContext.Tenants
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

    /// <inheritdoc />
    public async Task<IReadOnlyList<LeaseDto>> ListLeasesAsync(CancellationToken cancellationToken)
        => await dbContext.Leases
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

    /// <inheritdoc />
    public async Task<IReadOnlyList<LeaseTenantDto>> ListLeaseTenantsAsync(CancellationToken cancellationToken)
        => await dbContext.LeaseTenants
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
