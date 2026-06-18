using Loyeris.Shared.Results;
using Loyeris.TaxPreparation.App.Dtos;
using MediatR;

namespace Loyeris.TaxPreparation.App.Queries;

/// <summary>
/// Query that lists fiscal periods.
/// </summary>
public record GetFiscalPeriodsQuery() : IRequest<Result<IReadOnlyList<FiscalPeriodDto>>>;
