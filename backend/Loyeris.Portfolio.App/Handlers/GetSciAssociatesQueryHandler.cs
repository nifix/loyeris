using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Handles <see cref="GetSciAssociatesQuery"/>.
/// </summary>
public class GetSciAssociatesQueryHandler(IPortfolioReadRepository repository)
    : IRequestHandler<GetSciAssociatesQuery, Result<IReadOnlyList<SciAssociateDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<SciAssociateDto>>> Handle(GetSciAssociatesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<SciAssociateDto>>.Success(await repository.ListSciAssociatesAsync(cancellationToken));
}
