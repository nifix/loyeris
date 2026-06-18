using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Leasing.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="Tenant"/>.
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    /// <summary>
    /// Applies table, contact, status, and workspace list index settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(tenant => tenant.Id);
        builder.HasIndex(tenant => new { tenant.WorkspaceId, tenant.Status, tenant.LastName });
        builder.HasIndex(tenant => new { tenant.WorkspaceId, tenant.Email }).IsUnique().HasFilter("email IS NOT NULL");

        builder.Property(tenant => tenant.FirstName).HasMaxLength(120).IsRequired();
        builder.Property(tenant => tenant.LastName).HasMaxLength(120).IsRequired();
        builder.Property(tenant => tenant.Email).HasMaxLength(320);
        builder.Property(tenant => tenant.Phone).HasMaxLength(40);
        builder.Property(tenant => tenant.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(TenantStatus.Active);
        builder.Property(tenant => tenant.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(tenant => tenant.UpdatedAt).HasDefaultValueSql("now()");
    }
}
