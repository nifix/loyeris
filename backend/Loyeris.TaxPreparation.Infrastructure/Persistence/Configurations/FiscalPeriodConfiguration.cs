using Loyeris.TaxPreparation.Core.Entities;
using Loyeris.TaxPreparation.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.TaxPreparation.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="FiscalPeriod"/>.
/// </summary>
public class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
{
    /// <summary>
    /// Applies table, date range, status, and one-period-per-year uniqueness settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
    {
        builder.ToTable("fiscal_periods");
        builder.HasKey(period => period.Id);
        builder.HasIndex(period => new { period.SciId, period.Year }).IsUnique();

        builder.Property(period => period.StartsOn).HasColumnType("date");
        builder.Property(period => period.EndsOn).HasColumnType("date");
        builder.Property(period => period.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(FiscalPeriodStatus.Open);
        builder.Property(period => period.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(period => period.UpdatedAt).HasDefaultValueSql("now()");
    }
}
