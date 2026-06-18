using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Handles <see cref="GetScisQuery"/>.
/// </summary>
public class GetScisQueryHandler(IPortfolioReadRepository repository)
    : IRequestHandler<GetScisQuery, Result<IReadOnlyList<SciDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<SciDto>>> Handle(GetScisQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<SciDto>>.Success(await repository.ListScisAsync(cancellationToken));
}
