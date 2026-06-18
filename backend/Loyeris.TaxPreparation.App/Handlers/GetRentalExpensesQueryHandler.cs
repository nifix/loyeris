using Loyeris.Shared.Results;
using Loyeris.TaxPreparation.App.Dtos;
using Loyeris.TaxPreparation.App.Persistence;
using Loyeris.TaxPreparation.App.Queries;
using MediatR;

namespace Loyeris.TaxPreparation.App.Handlers;

/// <summary>
/// Handles <see cref="GetRentalExpensesQuery"/>.
/// </summary>
public class GetRentalExpensesQueryHandler(ITaxPreparationReadRepository repository)
    : IRequestHandler<GetRentalExpensesQuery, Result<IReadOnlyList<RentalExpenseDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<RentalExpenseDto>>> Handle(GetRentalExpensesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<RentalExpenseDto>>.Success(await repository.ListRentalExpensesAsync(cancellationToken));
}
