using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Portfolio.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Portfolio projections.
/// </summary>
public class PortfolioReadRepository(PortfolioDbContext dbContext) : IPortfolioReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<SciDto>> ListScisAsync(CancellationToken cancellationToken)
        => await dbContext.Scis
            .AsNoTracking()
            .OrderBy(sci => sci.Name)
            .Select(sci => new SciDto(
                sci.Id,
                sci.WorkspaceId,
                sci.Name,
                sci.Siren,
                sci.TaxRegime,
                sci.Status,
                sci.Street,
                sci.PostalCode,
                sci.City,
                sci.Country,
                sci.IncorporatedOn,
                sci.CreatedAt,
                sci.UpdatedAt,
                sci.ArchivedAt))
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<SciAssociateDto>> ListSciAssociatesAsync(CancellationToken cancellationToken)
        => await dbContext.SciAssociates
            .AsNoTracking()
            .OrderBy(associate => associate.LastName)
            .ThenBy(associate => associate.FirstName)
            .Select(associate => new SciAssociateDto(
                associate.Id,
                associate.SciId,
                associate.FirstName,
                associate.LastName,
                associate.Email,
                associate.SharesCount,
                associate.OwnershipPercentage,
                associate.CreatedAt,
                associate.UpdatedAt))
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<LotDto>> ListLotsAsync(CancellationToken cancellationToken)
        => await dbContext.Lots
            .AsNoTracking()
            .OrderBy(lot => lot.Reference)
            .Select(lot => new LotDto(
                lot.Id,
                lot.SciId,
                lot.Reference,
                lot.Type,
                lot.Status,
                lot.Street,
                lot.PostalCode,
                lot.City,
                lot.Country,
                lot.SurfaceSqm,
                lot.PotentialRentExcludingChargesCents,
                lot.PotentialChargesCents,
                lot.SuggestedDepositCents,
                lot.Notes,
                lot.CreatedAt,
                lot.UpdatedAt,
                lot.ArchivedAt))
            .ToListAsync(cancellationToken);
}
