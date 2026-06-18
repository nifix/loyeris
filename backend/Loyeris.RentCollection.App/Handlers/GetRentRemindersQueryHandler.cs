using Loyeris.RentCollection.App.Dtos;
using Loyeris.RentCollection.App.Persistence;
using Loyeris.RentCollection.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.RentCollection.App.Handlers;

/// <summary>
/// Handles <see cref="GetRentRemindersQuery"/>.
/// </summary>
public class GetRentRemindersQueryHandler(IRentCollectionReadRepository repository)
    : IRequestHandler<GetRentRemindersQuery, Result<IReadOnlyList<RentReminderDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<RentReminderDto>>> Handle(GetRentRemindersQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<RentReminderDto>>.Success(await repository.ListRemindersAsync(cancellationToken));
}
