using Loyeris.Messaging.App.Persistence;
using Loyeris.Messaging.Infrastructure.Persistence;
using Loyeris.Messaging.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loyeris.Messaging.Infrastructure;

/// <summary>
/// Registers Messaging infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Messaging persistence services to the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">The application configuration containing database connection strings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMessagingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyerisDatabase");

        services.AddDbContext<MessagingDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "messaging"));
        });

        services.AddScoped<IMessagingReadRepository, MessagingReadRepository>();

        return services;
    }
}
