using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Loyeris.Portfolio.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core write repository for SCI aggregates.
/// </summary>
public class SciRepository(PortfolioDbContext dbContext) : ISciRepository
{
    /// <inheritdoc />
    public Task<Sci> GetByIdAsync(Guid workspaceId, Guid sciId, CancellationToken cancellationToken)
    {
        return dbContext.Scis.SingleOrDefaultAsync(
            sci => sci.WorkspaceId == workspaceId && sci.Id == sciId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SciCreationPersistenceResult> CreateAsync(
        Sci sci,
        CancellationToken cancellationToken)
    {
        await dbContext.Scis.AddAsync(sci, cancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return SciCreationPersistenceResult.Created;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgresException)
        {
            if (postgresException.ConstraintName == "IX_scis_workspace_id_name")
                return SciCreationPersistenceResult.DuplicateName;

            if (postgresException.ConstraintName == "IX_scis_siren")
                return SciCreationPersistenceResult.DuplicateSiren;

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<SciUpdatePersistenceResult> UpdateAsync(
        Sci sci,
        CancellationToken cancellationToken)
    {
        dbContext.Scis.Update(sci);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return SciUpdatePersistenceResult.Updated;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgresException)
        {
            if (postgresException.ConstraintName == "IX_scis_workspace_id_name")
                return SciUpdatePersistenceResult.DuplicateName;

            if (postgresException.ConstraintName == "IX_scis_siren")
                return SciUpdatePersistenceResult.DuplicateSiren;

            throw;
        }
    }
}
