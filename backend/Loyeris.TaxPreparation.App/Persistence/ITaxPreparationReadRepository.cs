using Loyeris.TaxPreparation.App.Dtos;

namespace Loyeris.TaxPreparation.App.Persistence;

/// <summary>
/// Provides read-only projections for Tax Preparation screens and endpoints.
/// </summary>
public interface ITaxPreparationReadRepository
{
    /// <summary>
    /// Lists fiscal periods.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The fiscal periods.</returns>
    Task<IReadOnlyList<FiscalPeriodDto>> ListFiscalPeriodsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists rental expenses.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The rental expenses.</returns>
    Task<IReadOnlyList<RentalExpenseDto>> ListRentalExpensesAsync(CancellationToken cancellationToken);
}
