using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Notifications;
using Loyeris.IdentityAccess.App.Security;
using Loyeris.IdentityAccess.Infrastructure.Persistence;
using Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;
using Loyeris.IdentityAccess.Infrastructure.Notifications;
using Loyeris.IdentityAccess.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        services.AddScoped<IAccountRegistrationRepository, AccountRegistrationRepository>();
        services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuthOneTimeTokenRepository, AuthOneTimeTokenRepository>();
        services.AddScoped<IAuthEventRepository, AuthEventRepository>();
        services.AddScoped<IIdentityAccessUnitOfWork, IdentityAccessUnitOfWork>();
        services.AddScoped<IIdentityAccessReadRepository, IdentityAccessReadRepository>();
        services.AddSingleton<IAccountPasswordHasher, AccountPasswordHasher>();
        services.AddSingleton<IOneTimeTokenService, OneTimeTokenService>();
        services.AddSingleton<IEmailVerificationSender, SmtpEmailVerificationSender>();
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
