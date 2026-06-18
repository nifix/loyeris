using Loyeris.Portfolio.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Portfolio.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="SciAssociate"/>.
/// </summary>
public class SciAssociateConfiguration : IEntityTypeConfiguration<SciAssociate>
{
    /// <summary>
    /// Applies table, ownership precision, and SCI relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<SciAssociate> builder)
    {
        builder.ToTable("sci_associates");
        builder.HasKey(associate => associate.Id);
        builder.HasIndex(associate => associate.SciId);

        builder.Property(associate => associate.FirstName).HasMaxLength(120);
        builder.Property(associate => associate.LastName).HasMaxLength(120).IsRequired();
        builder.Property(associate => associate.Email).HasMaxLength(320);
        builder.Property(associate => associate.OwnershipPercentage).HasPrecision(5, 2);
        builder.Property(associate => associate.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(associate => associate.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(associate => associate.Sci)
            .WithMany(sci => sci.Associates)
            .HasForeignKey(associate => associate.SciId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
