namespace Loyeris.Api.Contracts.Leasing;

/// <summary>
/// HTTP payload used to update an existing lease without changing its tenant.
/// </summary>
public record UpdateLotLeaseRequest(
    DateOnly StartsOn,
    DateOnly? EndsOn,
    int RentDueDay,
    long RentExcludingChargesCents,
    long ChargesCents,
    long DepositCents,
    string PaymentTerms,
    string Notes);
