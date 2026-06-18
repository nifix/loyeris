using Loyeris.Portfolio.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Portfolio.Infrastructure.Persistence;

/// <summary>
/// EF Core context responsible for SCI, associate, and lot persistence.
/// </summary>
public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the rentable lots owned by SCI structures.
    /// </summary>
    public DbSet<Lot> Lots => Set<Lot>();

    /// <summary>
    /// Gets the SCI structures managed in the portfolio.
    /// </summary>
    public DbSet<Sci> Scis => Set<Sci>();

    /// <summary>
    /// Gets the associates linked to SCI structures.
    /// </summary>
    public DbSet<SciAssociate> SciAssociates => Set<SciAssociate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("portfolio");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortfolioDbContext).Assembly);
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
