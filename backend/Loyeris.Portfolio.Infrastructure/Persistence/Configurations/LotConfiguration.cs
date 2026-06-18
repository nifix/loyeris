using Loyeris.Portfolio.Core.Entities;
using Loyeris.Portfolio.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="Lot"/>.
/// </summary>
public class LotConfiguration : IEntityTypeConfiguration<Lot>
{
    /// <summary>
    /// Applies table, address, rent potential, and SCI relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<Lot> builder)
    {
        builder.ToTable("lots");
        builder.HasKey(lot => lot.Id);
        builder.HasIndex(lot => new { lot.SciId, lot.Reference }).IsUnique();
        builder.HasIndex(lot => new { lot.SciId, lot.Status });

        builder.Property(lot => lot.Reference).HasMaxLength(80).IsRequired();
        builder.Property(lot => lot.Type).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(lot => lot.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(LotStatus.Active);
        builder.Property(lot => lot.Street).HasMaxLength(180).IsRequired();
        builder.Property(lot => lot.PostalCode).HasMaxLength(20).IsRequired();
        builder.Property(lot => lot.City).HasMaxLength(120).IsRequired();
        builder.Property(lot => lot.Country).HasMaxLength(2).HasDefaultValue("FR").IsRequired();
        builder.Property(lot => lot.SurfaceSqm).HasPrecision(7, 2);
        builder.Property(lot => lot.PotentialRentExcludingChargesCents).HasDefaultValue(0L);
        builder.Property(lot => lot.PotentialChargesCents).HasDefaultValue(0L);
        builder.Property(lot => lot.SuggestedDepositCents).HasDefaultValue(0L);
        builder.Property(lot => lot.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(lot => lot.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(lot => lot.Sci)
            .WithMany(sci => sci.Lots)
            .HasForeignKey(lot => lot.SciId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
