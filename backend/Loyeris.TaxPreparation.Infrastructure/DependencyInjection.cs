using Loyeris.TaxPreparation.App.Persistence;
using Loyeris.TaxPreparation.Infrastructure.Persistence;
using Loyeris.TaxPreparation.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loyeris.TaxPreparation.Infrastructure;

/// <summary>
/// Registers Tax Preparation infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Tax Preparation persistence services to the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">The application configuration containing database connection strings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddTaxPreparationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyerisDatabase");

        services.AddDbContext<TaxPreparationDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "tax"));
        });

        services.AddScoped<ITaxPreparationReadRepository, TaxPreparationReadRepository>();

        return services;
    }
}
