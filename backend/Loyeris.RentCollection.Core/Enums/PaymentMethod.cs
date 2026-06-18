namespace Loyeris.RentCollection.Core.Enums;

/// <summary>
/// Identifies how a rent payment was received.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Payment received by bank transfer.
    /// </summary>
    BankTransfer = 0,

    /// <summary>
    /// Payment received by card.
    /// </summary>
    Card = 1,

    /// <summary>
    /// Payment received in cash.
    /// </summary>
    Cash = 2,

    /// <summary>
    /// Payment received by check.
    /// </summary>
    Check = 3,

    /// <summary>
    /// Payment received through another method.
    /// </summary>
    Other = 4
}
