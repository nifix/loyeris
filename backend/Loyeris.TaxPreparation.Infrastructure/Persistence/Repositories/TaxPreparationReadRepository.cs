using Loyeris.TaxPreparation.App.Dtos;
using Loyeris.TaxPreparation.App.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.TaxPreparation.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Tax Preparation projections.
/// </summary>
public class TaxPreparationReadRepository(TaxPreparationDbContext dbContext) : ITaxPreparationReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<FiscalPeriodDto>> ListFiscalPeriodsAsync(CancellationToken cancellationToken)
        => await dbContext.FiscalPeriods
            .AsNoTracking()
            .OrderByDescending(period => period.Year)
            .Select(period => new FiscalPeriodDto(
                period.Id,
                period.SciId,
                period.Year,
                period.StartsOn,
                period.EndsOn,
                period.Status,
                period.CreatedAt,
                period.UpdatedAt))
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<RentalExpenseDto>> ListRentalExpensesAsync(CancellationToken cancellationToken)
        => await dbContext.RentalExpenses
            .AsNoTracking()
            .OrderByDescending(expense => expense.ExpenseDate)
            .Select(expense => new RentalExpenseDto(
                expense.Id,
                expense.SciId,
                expense.LotId,
                expense.ExpenseDate,
                expense.Category,
                expense.Label,
                expense.AmountCents,
                expense.DeductibleForIr,
                expense.DocumentUrl,
                expense.Note,
                expense.CreatedAt,
                expense.UpdatedAt))
            .ToListAsync(cancellationToken);
}
