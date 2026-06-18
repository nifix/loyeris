using Loyeris.Messaging.App.Dtos;
using Loyeris.Messaging.App.Persistence;
using Loyeris.Messaging.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Messaging.App.Handlers;

/// <summary>
/// Handles <see cref="GetNotificationPreferencesQuery"/>.
/// </summary>
public class GetNotificationPreferencesQueryHandler(IMessagingReadRepository repository)
    : IRequestHandler<GetNotificationPreferencesQuery, Result<IReadOnlyList<NotificationPreferenceDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<NotificationPreferenceDto>>> Handle(GetNotificationPreferencesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<NotificationPreferenceDto>>.Success(await repository.ListNotificationPreferencesAsync(cancellationToken));
}
