using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Infrastructure.Persistence;
using Loyeris.Leasing.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loyeris.Leasing.Infrastructure;

/// <summary>
/// Registers Leasing infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Leasing persistence services to the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">The application configuration containing database connection strings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddLeasingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyerisDatabase");

        services.AddDbContext<LeasingDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "leasing")));

        services.AddScoped<ILeasingReadRepository, LeasingReadRepository>();

        return services;
    }
}
