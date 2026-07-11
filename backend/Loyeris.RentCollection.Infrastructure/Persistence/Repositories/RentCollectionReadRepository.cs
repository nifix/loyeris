using Loyeris.RentCollection.App.Dtos;
using Loyeris.RentCollection.App.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.RentCollection.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core read repository for Rent Collection projections.
/// </summary>
public class RentCollectionReadRepository(RentCollectionDbContext dbContext) : IRentCollectionReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RentDeadlineDto>> ListDeadlinesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.RentDeadlines
            .AsNoTracking()
            .OrderByDescending(deadline => deadline.PeriodMonth)
            .ThenBy(deadline => deadline.DueOn)
            .Select(deadline => new RentDeadlineDto(
                deadline.Id,
                deadline.LeaseId,
                deadline.PeriodMonth,
                deadline.DueOn,
                deadline.RentExcludingChargesCents,
                deadline.ChargesCents,
                deadline.TotalDueCents,
                deadline.PaidCents,
                deadline.RemainingCents,
                deadline.Status,
                deadline.GeneratedAt,
                deadline.CreatedAt,
                deadline.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RentPaymentDto>> ListPaymentsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.RentPayments
            .AsNoTracking()
            .OrderByDescending(payment => payment.PaidOn)
            .Select(payment => new RentPaymentDto(
                payment.Id,
                payment.RentDeadlineId,
                payment.AmountCents,
                payment.PaidOn,
                payment.RecordedByUserId,
                payment.Method,
                payment.Reference,
                payment.Note,
                payment.CreatedAt,
                payment.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RentReminderDto>> ListRemindersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.RentReminders
            .AsNoTracking()
            .OrderByDescending(reminder => reminder.ScheduledFor ?? reminder.CreatedAt)
            .Select(reminder => new RentReminderDto(
                reminder.Id,
                reminder.RentDeadlineId,
                reminder.Channel,
                reminder.Status,
                reminder.ScheduledFor,
                reminder.SentAt,
                reminder.RecipientEmail,
                reminder.Subject,
                reminder.Body,
                reminder.CreatedAt,
                reminder.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
