using Loyeris.Messaging.App.Dtos;

namespace Loyeris.Messaging.App.Persistence;

/// <summary>
/// Provides read-only projections for Messaging screens and endpoints.
/// </summary>
public interface IMessagingReadRepository
{
    /// <summary>
    /// Lists notification preferences.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The notification preferences.</returns>
    Task<IReadOnlyList<NotificationPreferenceDto>> ListNotificationPreferencesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists outbox messages without payload content.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The outbox messages.</returns>
    Task<IReadOnlyList<OutboxMessageDto>> ListOutboxMessagesAsync(CancellationToken cancellationToken);
}
