using Loyeris.RentCollection.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.RentCollection.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="RentPayment"/>.
/// </summary>
public class RentPaymentConfiguration : IEntityTypeConfiguration<RentPayment>
{
    /// <summary>
    /// Applies table, payment method, history index, and deadline relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<RentPayment> builder)
    {
        builder.ToTable("rent_payments");
        builder.HasKey(payment => payment.Id);
        builder.HasIndex(payment => new { payment.RentDeadlineId, payment.PaidOn });
        builder.HasIndex(payment => payment.PaidOn).IsDescending();

        builder.Property(payment => payment.PaidOn).HasColumnType("date");
        builder.Property(payment => payment.Method).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(payment => payment.Reference).HasMaxLength(120);
        builder.Property(payment => payment.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(payment => payment.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(payment => payment.RentDeadline)
            .WithMany(deadline => deadline.Payments)
            .HasForeignKey(payment => payment.RentDeadlineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
