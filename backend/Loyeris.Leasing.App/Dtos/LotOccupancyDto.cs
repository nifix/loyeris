namespace Loyeris.Leasing.App.Dtos;

/// <summary>
/// Active lease and primary tenant currently attached to a lot.
/// </summary>
public record LotOccupancyDto(
    Guid LotId,
    Guid LeaseId,
    Guid TenantId,
    string TenantFirstName,
    string TenantLastName,
    DateOnly StartsOn,
    DateOnly? EndsOn,
    int RentDueDay,
    long RentExcludingChargesCents,
    long ChargesCents,
    long DepositCents,
    string PaymentTerms,
    string Notes);
