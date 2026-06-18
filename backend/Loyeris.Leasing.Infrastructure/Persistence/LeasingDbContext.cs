using Loyeris.Leasing.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Leasing.Infrastructure.Persistence;

/// <summary>
/// EF Core context responsible for tenants, leases, and occupancy history.
/// </summary>
public class LeasingDbContext(DbContextOptions<LeasingDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the leases that drive occupancy and rent generation.
    /// </summary>
    public DbSet<Lease> Leases => Set<Lease>();

    /// <summary>
    /// Gets the links between leases and their tenants or guarantors.
    /// </summary>
    public DbSet<LeaseTenant> LeaseTenants => Set<LeaseTenant>();

    /// <summary>
    /// Gets the tenant records managed in the workspace.
    /// </summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("leasing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeasingDbContext).Assembly);
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
