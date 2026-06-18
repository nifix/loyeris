using Loyeris.RentCollection.Core.Entities;
using Loyeris.RentCollection.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.RentCollection.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="RentDeadline"/>.
/// </summary>
public class RentDeadlineConfiguration : IEntityTypeConfiguration<RentDeadline>
{
    /// <summary>
    /// Applies table, monthly uniqueness, status, and operational dashboard index settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<RentDeadline> builder)
    {
        builder.ToTable("rent_deadlines");
        builder.HasKey(deadline => deadline.Id);
        builder.HasIndex(deadline => new { deadline.LeaseId, deadline.PeriodMonth }).IsUnique();
        builder.HasIndex(deadline => new { deadline.PeriodMonth, deadline.Status });
        builder.HasIndex(deadline => new { deadline.DueOn, deadline.Status });

        builder.Property(deadline => deadline.PeriodMonth).HasColumnType("date");
        builder.Property(deadline => deadline.DueOn).HasColumnType("date");
        builder.Property(deadline => deadline.PaidCents).HasDefaultValue(0L);
        builder.Property(deadline => deadline.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(RentDeadlineStatus.Pending);
        builder.Property(deadline => deadline.GeneratedAt).HasDefaultValueSql("now()");
        builder.Property(deadline => deadline.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(deadline => deadline.UpdatedAt).HasDefaultValueSql("now()");
    }
}
