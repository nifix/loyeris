using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Leasing.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="LeaseTenant"/>.
/// </summary>
public class LeaseTenantConfiguration : IEntityTypeConfiguration<LeaseTenant>
{
    /// <summary>
    /// Applies table, role, uniqueness, and internal leasing relationships.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<LeaseTenant> builder)
    {
        builder.ToTable("lease_tenants");
        builder.HasKey(leaseTenant => leaseTenant.Id);
        builder.HasIndex(leaseTenant => new { leaseTenant.LeaseId, leaseTenant.TenantId }).IsUnique();
        builder.HasIndex(leaseTenant => leaseTenant.TenantId);

        builder.Property(leaseTenant => leaseTenant.Role).HasConversion<string>().HasMaxLength(30).HasDefaultValue(LeaseTenantRole.Primary);
        builder.Property(leaseTenant => leaseTenant.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(leaseTenant => leaseTenant.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(leaseTenant => leaseTenant.Lease)
            .WithMany(lease => lease.LeaseTenants)
            .HasForeignKey(leaseTenant => leaseTenant.LeaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(leaseTenant => leaseTenant.Tenant)
            .WithMany(tenant => tenant.LeaseTenants)
            .HasForeignKey(leaseTenant => leaseTenant.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
