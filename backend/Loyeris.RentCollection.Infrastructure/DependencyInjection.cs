using Loyeris.RentCollection.App.Persistence;
using Loyeris.RentCollection.Infrastructure.Persistence;
using Loyeris.RentCollection.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loyeris.RentCollection.Infrastructure;

/// <summary>
/// Registers Rent Collection infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Rent Collection persistence services to the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">The application configuration containing database connection strings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRentCollectionInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyerisDatabase");

        services.AddDbContext<RentCollectionDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "collection")));

        services.AddScoped<IRentCollectionReadRepository, RentCollectionReadRepository>();

        return services;
    }
}
