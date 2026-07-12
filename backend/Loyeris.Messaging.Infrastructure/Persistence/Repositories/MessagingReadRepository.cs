using Loyeris.Messaging.App.Dtos;
using Loyeris.Messaging.App.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.Messaging.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Messaging projections.
/// </summary>
public class MessagingReadRepository(MessagingDbContext dbContext) : IMessagingReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationPreferenceDto>> ListNotificationPreferencesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.NotificationPreferences
            .AsNoTracking()
            .OrderBy(preference => preference.UserId)
            .Select(preference => new NotificationPreferenceDto(
                preference.Id,
                preference.UserId,
                preference.RentOverdueEmailEnabled,
                preference.PaymentRecordedEmailEnabled,
                preference.LeaseEndingEmailEnabled,
                preference.LeaseEndingNoticeMonths,
                preference.CreatedAt,
                preference.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OutboxMessageDto>> ListOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.OutboxMessages
            .AsNoTracking()
            .OrderBy(message => message.Status)
            .ThenBy(message => message.AvailableAt)
            .Select(message => new OutboxMessageDto(
                message.Id,
                message.Type,
                message.Status,
                message.AvailableAt,
                message.ProcessedAt,
                message.Error,
                message.CreatedAt,
                message.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
