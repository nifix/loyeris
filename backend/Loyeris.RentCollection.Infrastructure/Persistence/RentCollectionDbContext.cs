using Loyeris.RentCollection.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.RentCollection.Infrastructure.Persistence;

/// <summary>
/// EF Core context responsible for generated rent deadlines, payments, and reminders.
/// </summary>
public class RentCollectionDbContext(DbContextOptions<RentCollectionDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets monthly rent deadlines generated from leases.
    /// </summary>
    public DbSet<RentDeadline> RentDeadlines => Set<RentDeadline>();

    /// <summary>
    /// Gets payments recorded against rent deadlines.
    /// </summary>
    public DbSet<RentPayment> RentPayments => Set<RentPayment>();

    /// <summary>
    /// Gets reminder actions linked to unpaid rent deadlines.
    /// </summary>
    public DbSet<RentReminder> RentReminders => Set<RentReminder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("collection");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentCollectionDbContext).Assembly);
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
