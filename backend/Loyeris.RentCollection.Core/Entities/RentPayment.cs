using Loyeris.RentCollection.Core.Enums;

namespace Loyeris.RentCollection.Core.Entities;

/// <summary>
/// Represents a payment received for a rent deadline.
/// </summary>
public class RentPayment
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the payment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the rent deadline paid by this payment.
    /// </summary>
    public Guid RentDeadlineId { get; set; }

    /// <summary>
    /// Gets or sets the payment amount, stored in cents.
    /// </summary>
    public long AmountCents { get; set; }

    /// <summary>
    /// Gets or sets the date on which the payment was received.
    /// </summary>
    public DateOnly PaidOn { get; set; }

    /// <summary>
    /// Gets or sets the user who recorded the payment.
    /// </summary>
    public Guid RecordedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the method used to receive the payment.
    /// </summary>
    public PaymentMethod Method { get; set; }

    /// <summary>
    /// Gets or sets an optional banking or check reference.
    /// </summary>
    public string Reference { get; set; }

    /// <summary>
    /// Gets or sets an optional note entered when recording the payment.
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the rent deadline navigation.
    /// </summary>
    public RentDeadline RentDeadline { get; set; }
}
