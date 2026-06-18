using Loyeris.TaxPreparation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.TaxPreparation.Infrastructure.Persistence;

/// <summary>
/// EF Core context responsible for tax preparation data such as expenses and fiscal periods.
/// </summary>
public class TaxPreparationDbContext(DbContextOptions<TaxPreparationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the fiscal periods prepared for SCI structures.
    /// </summary>
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();

    /// <summary>
    /// Gets expenses used for rental and SCI tax preparation.
    /// </summary>
    public DbSet<RentalExpense> RentalExpenses => Set<RentalExpense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tax");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaxPreparationDbContext).Assembly);
        ApplySnakeCaseColumnNames(modelBuilder);
    }

    private static void ApplySnakeCaseColumnNames(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var chars = new List<char>(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];

            if (char.IsUpper(current) && i > 0)
            {
                var previous = value[i - 1];
                var hasNext = i + 1 < value.Length;

                if (previous != '_' && (!char.IsUpper(previous) || hasNext && !char.IsUpper(value[i + 1])))
                {
                    chars.Add('_');
                }
            }

            chars.Add(char.ToLowerInvariant(current));
        }

        return new string(chars.ToArray());
    }
}
