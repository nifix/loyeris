using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.Infrastructure.Persistence;
using Loyeris.Portfolio.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Loyeris.Portfolio.Infrastructure;

/// <summary>
/// Registers Portfolio infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Portfolio persistence services to the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">The application configuration containing database connection strings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPortfolioInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyerisDatabase");

        services.AddDbContext<PortfolioDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "portfolio"));
        });

        services.AddScoped<IPortfolioReadRepository, PortfolioReadRepository>();
        services.AddScoped<ISciRepository, SciRepository>();
        services.AddScoped<ILotRepository, LotRepository>();
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
