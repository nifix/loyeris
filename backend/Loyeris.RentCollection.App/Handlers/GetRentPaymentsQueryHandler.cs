using Loyeris.RentCollection.App.Dtos;
using Loyeris.RentCollection.App.Persistence;
using Loyeris.RentCollection.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.RentCollection.App.Handlers;

/// <summary>
/// Handles <see cref="GetRentPaymentsQuery"/>.
/// </summary>
public class GetRentPaymentsQueryHandler(IRentCollectionReadRepository repository)
    : IRequestHandler<GetRentPaymentsQuery, Result<IReadOnlyList<RentPaymentDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<RentPaymentDto>>> Handle(GetRentPaymentsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<RentPaymentDto>>.Success(await repository.ListPaymentsAsync(cancellationToken));
}
