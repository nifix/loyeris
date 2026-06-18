using Loyeris.RentCollection.App.Dtos;

namespace Loyeris.RentCollection.App.Persistence;

/// <summary>
/// Provides read-only projections for Rent Collection screens and endpoints.
/// </summary>
public interface IRentCollectionReadRepository
{
    /// <summary>
    /// Lists rent deadlines.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The rent deadlines.</returns>
    Task<IReadOnlyList<RentDeadlineDto>> ListDeadlinesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists rent payments.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The rent payments.</returns>
    Task<IReadOnlyList<RentPaymentDto>> ListPaymentsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists rent reminders.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The rent reminders.</returns>
    Task<IReadOnlyList<RentReminderDto>> ListRemindersAsync(CancellationToken cancellationToken);
}
