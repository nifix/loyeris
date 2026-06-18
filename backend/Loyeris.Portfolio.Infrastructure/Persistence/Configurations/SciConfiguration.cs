using Loyeris.Portfolio.Core.Entities;
using Loyeris.Portfolio.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="Sci"/>.
/// </summary>
public class SciConfiguration : IEntityTypeConfiguration<Sci>
{
    /// <summary>
    /// Applies table, legal identifier, tax regime, and workspace indexes.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<Sci> builder)
    {
        builder.ToTable("scis");
        builder.HasKey(sci => sci.Id);
        builder.HasIndex(sci => new { sci.WorkspaceId, sci.Name }).IsUnique();
        builder.HasIndex(sci => sci.Siren).IsUnique().HasFilter("siren IS NOT NULL");
        builder.HasIndex(sci => new { sci.WorkspaceId, sci.Status });

        builder.Property(sci => sci.Name).HasMaxLength(180).IsRequired();
        builder.Property(sci => sci.Siren).HasMaxLength(9);
        builder.Property(sci => sci.TaxRegime).HasConversion<string>().HasMaxLength(20).HasDefaultValue(TaxRegime.IR);
        builder.Property(sci => sci.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(SciStatus.Active);
        builder.Property(sci => sci.Street).HasMaxLength(180);
        builder.Property(sci => sci.PostalCode).HasMaxLength(20);
        builder.Property(sci => sci.City).HasMaxLength(120);
        builder.Property(sci => sci.Country).HasMaxLength(2).HasDefaultValue("FR").IsRequired();
        builder.Property(sci => sci.IncorporatedOn).HasColumnType("date");
        builder.Property(sci => sci.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(sci => sci.UpdatedAt).HasDefaultValueSql("now()");
    }
}
