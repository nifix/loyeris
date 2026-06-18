using Loyeris.TaxPreparation.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.TaxPreparation.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="RentalExpense"/>.
/// </summary>
public class RentalExpenseConfiguration : IEntityTypeConfiguration<RentalExpense>
{
    /// <summary>
    /// Applies table, categorization, deductible flag, and SCI/lot date index settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<RentalExpense> builder)
    {
        builder.ToTable("rental_expenses");
        builder.HasKey(expense => expense.Id);
        builder.HasIndex(expense => new { expense.SciId, expense.ExpenseDate });
        builder.HasIndex(expense => new { expense.LotId, expense.ExpenseDate });

        builder.Property(expense => expense.ExpenseDate).HasColumnType("date");
        builder.Property(expense => expense.Category).HasMaxLength(80).IsRequired();
        builder.Property(expense => expense.Label).HasMaxLength(180).IsRequired();
        builder.Property(expense => expense.DeductibleForIr).HasDefaultValue(true);
        builder.Property(expense => expense.DocumentUrl);
        builder.Property(expense => expense.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(expense => expense.UpdatedAt).HasDefaultValueSql("now()");
    }
}
