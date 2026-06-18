using Loyeris.Messaging.App.Dtos;
using Loyeris.Messaging.App.Persistence;
using Loyeris.Messaging.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Messaging.App.Handlers;

/// <summary>
/// Handles <see cref="GetOutboxMessagesQuery"/>.
/// </summary>
public class GetOutboxMessagesQueryHandler(IMessagingReadRepository repository)
    : IRequestHandler<GetOutboxMessagesQuery, Result<IReadOnlyList<OutboxMessageDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<OutboxMessageDto>>> Handle(GetOutboxMessagesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<OutboxMessageDto>>.Success(await repository.ListOutboxMessagesAsync(cancellationToken));
}
