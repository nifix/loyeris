using Loyeris.RentCollection.App.Dtos;
using Loyeris.RentCollection.App.Persistence;
using Loyeris.RentCollection.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.RentCollection.App.Handlers;

/// <summary>
/// Handles <see cref="GetRentDeadlinesQuery"/>.
/// </summary>
public class GetRentDeadlinesQueryHandler(IRentCollectionReadRepository repository)
    : IRequestHandler<GetRentDeadlinesQuery, Result<IReadOnlyList<RentDeadlineDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<RentDeadlineDto>>> Handle(GetRentDeadlinesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<RentDeadlineDto>>.Success(await repository.ListDeadlinesAsync(cancellationToken));
}
