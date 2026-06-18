using Loyeris.Shared.Results;
using Loyeris.TaxPreparation.App.Dtos;
using Loyeris.TaxPreparation.App.Persistence;
using Loyeris.TaxPreparation.App.Queries;
using MediatR;

namespace Loyeris.TaxPreparation.App.Handlers;

/// <summary>
/// Handles <see cref="GetFiscalPeriodsQuery"/>.
/// </summary>
public class GetFiscalPeriodsQueryHandler(ITaxPreparationReadRepository repository)
    : IRequestHandler<GetFiscalPeriodsQuery, Result<IReadOnlyList<FiscalPeriodDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<FiscalPeriodDto>>> Handle(GetFiscalPeriodsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<FiscalPeriodDto>>.Success(await repository.ListFiscalPeriodsAsync(cancellationToken));
}
