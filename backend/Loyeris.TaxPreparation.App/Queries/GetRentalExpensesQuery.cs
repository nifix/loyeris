using Loyeris.Shared.Results;
using Loyeris.TaxPreparation.App.Dtos;
using MediatR;

namespace Loyeris.TaxPreparation.App.Queries;

/// <summary>
/// Query that lists rental expenses.
/// </summary>
public record GetRentalExpensesQuery() : IRequest<Result<IReadOnlyList<RentalExpenseDto>>>;
