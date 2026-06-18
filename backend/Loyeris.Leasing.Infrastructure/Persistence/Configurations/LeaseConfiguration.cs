using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Leasing.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="Lease"/>.
/// </summary>
public class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    /// <summary>
    /// Applies table, rent terms, lease dates, and active-lease uniqueness settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<Lease> builder)
    {
        builder.ToTable("leases");
        builder.HasKey(lease => lease.Id);
        builder.HasIndex(lease => new { lease.LotId, lease.Status });
        builder.HasIndex(lease => new { lease.StartsOn, lease.EndsOn });
        builder.HasIndex(lease => lease.LotId).IsUnique().HasFilter("status = 'Active'");

        builder.Property(lease => lease.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(LeaseStatus.Draft);
        builder.Property(lease => lease.StartsOn).HasColumnType("date");
        builder.Property(lease => lease.EndsOn).HasColumnType("date");
        builder.Property(lease => lease.RentDueDay).HasDefaultValue(5);
        builder.Property(lease => lease.PaymentTerms);
        builder.Property(lease => lease.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(lease => lease.UpdatedAt).HasDefaultValueSql("now()");
    }
}
