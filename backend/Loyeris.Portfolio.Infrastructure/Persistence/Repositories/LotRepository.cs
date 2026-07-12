using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Loyeris.Portfolio.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core write repository for rental lots.
/// </summary>
public class LotRepository(PortfolioDbContext dbContext) : ILotRepository
{
    /// <inheritdoc />
    public Task<Lot> GetByIdAsync(Guid workspaceId, Guid lotId, CancellationToken cancellationToken)
    {
        return dbContext.Lots
            .Include(lot => lot.Sci)
            .SingleOrDefaultAsync(
                lot => lot.Id == lotId && lot.Sci.WorkspaceId == workspaceId,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<Sci> GetSciByIdAsync(Guid workspaceId, Guid sciId, CancellationToken cancellationToken)
    {
        return dbContext.Scis.SingleOrDefaultAsync(
            sci => sci.Id == sciId && sci.WorkspaceId == workspaceId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LotPersistenceResult> CreateAsync(Lot lot, CancellationToken cancellationToken)
    {
        await dbContext.Lots.AddAsync(lot, cancellationToken);
        return await SaveAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LotPersistenceResult> UpdateAsync(Lot lot, CancellationToken cancellationToken)
    {
        dbContext.Lots.Update(lot);
        return await SaveAsync(cancellationToken);
    }

    private async Task<LotPersistenceResult> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return LotPersistenceResult.Saved;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException { ConstraintName: "IX_lots_sci_id_reference" })
        {
            return LotPersistenceResult.DuplicateReference;
        }
    }
}
