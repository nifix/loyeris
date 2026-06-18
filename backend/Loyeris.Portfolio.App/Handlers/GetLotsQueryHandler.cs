using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Handles <see cref="GetLotsQuery"/>.
/// </summary>
public class GetLotsQueryHandler(IPortfolioReadRepository repository)
    : IRequestHandler<GetLotsQuery, Result<IReadOnlyList<LotDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<LotDto>>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<LotDto>>.Success(await repository.ListLotsAsync(cancellationToken));
}
