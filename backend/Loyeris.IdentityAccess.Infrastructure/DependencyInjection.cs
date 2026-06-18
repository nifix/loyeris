using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Infrastructure.Persistence;
using Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loyeris.IdentityAccess.Infrastructure;

/// <summary>
/// Registers Identity Access infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Identity Access persistence services to the service collection.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">The application configuration containing database connection strings.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddIdentityAccessInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyerisDatabase");

        services.AddDbContext<IdentityAccessDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "identity")));

        services.AddScoped<IAppUserAuthRepository, AppUserAuthRepository>();
        services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuthOneTimeTokenRepository, AuthOneTimeTokenRepository>();
        services.AddScoped<IAuthEventRepository, AuthEventRepository>();
        services.AddScoped<IIdentityAccessUnitOfWork, IdentityAccessUnitOfWork>();
        services.AddScoped<IIdentityAccessReadRepository, IdentityAccessReadRepository>();

        return services;
    }
}
