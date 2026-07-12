using Loyeris.IdentityAccess.Infrastructure.Persistence;
using Loyeris.Leasing.Infrastructure.Persistence;
using Loyeris.Messaging.Infrastructure.Persistence;
using Loyeris.Portfolio.Infrastructure.Persistence;
using Loyeris.RentCollection.Infrastructure.Persistence;
using Loyeris.TaxPreparation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Api.Operations;

/// <summary>
/// Applies every domain migration as an explicit operational command.
/// </summary>
public static class DatabaseMigrator
{
    /// <summary>
    /// Applies all domain migrations in a stable order.
    /// </summary>
    public static async Task MigrateAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var scopedServices = scope.ServiceProvider;

        var contexts = new (string Name, DbContext Context)[]
        {
            ("IdentityAccess", scopedServices.GetRequiredService<IdentityAccessDbContext>()),
            ("Portfolio", scopedServices.GetRequiredService<PortfolioDbContext>()),
            ("Leasing", scopedServices.GetRequiredService<LeasingDbContext>()),
            ("RentCollection", scopedServices.GetRequiredService<RentCollectionDbContext>()),
            ("TaxPreparation", scopedServices.GetRequiredService<TaxPreparationDbContext>()),
            ("Messaging", scopedServices.GetRequiredService<MessagingDbContext>())
        };

        foreach (var (name, context) in contexts)
        {
            logger.LogInformation("Applying {Domain} database migrations.", name);
            await context.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Applied {Domain} database migrations successfully.", name);
        }
    }
}
