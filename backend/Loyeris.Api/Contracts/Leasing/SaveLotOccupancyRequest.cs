namespace Loyeris.Api.Contracts.Leasing;

/// <summary>
/// HTTP payload used to save the active occupancy of a rental lot.
/// </summary>
public record SaveLotOccupancyRequest(
    Guid? TenantId,
    DateOnly? StartsOn,
    DateOnly? EndsOn,
    int RentDueDay,
    long RentExcludingChargesCents,
    long ChargesCents,
    long DepositCents,
    string PaymentTerms,
    string Notes);
