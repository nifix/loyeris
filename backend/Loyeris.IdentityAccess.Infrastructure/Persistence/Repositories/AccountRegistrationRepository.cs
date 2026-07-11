using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Repositories;

/// <summary>
/// Persists the complete self-service account registration graph.
/// </summary>
public class AccountRegistrationRepository(IdentityAccessDbContext dbContext) : IAccountRegistrationRepository
{
    /// <inheritdoc />
    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken)
        => dbContext.AppUsers.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

    /// <inheritdoc />
    public async Task<AccountRegistrationPersistenceResult> CreateAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        await dbContext.AppUsers.AddAsync(user, cancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return AccountRegistrationPersistenceResult.Created;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "IX_app_users_normalized_email"
            })
        {
            // Another request may insert the same normalized email after the application-level pre-check.
            dbContext.ChangeTracker.Clear();
            return AccountRegistrationPersistenceResult.DuplicateEmail;
        }
    }
}
