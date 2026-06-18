using Loyeris.Messaging.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Messaging.Infrastructure.Persistence;

/// <summary>
/// EF Core context responsible for notification preferences and outbox messages.
/// </summary>
public class MessagingDbContext(DbContextOptions<MessagingDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets user notification preferences.
    /// </summary>
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    /// <summary>
    /// Gets reliable asynchronous messages waiting for background processing.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("messaging");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagingDbContext).Assembly);
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
