using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Commands;

/// <summary>
/// Creates, updates, replaces, or ends the active occupancy of a rental lot.
/// </summary>
public record SaveLotOccupancyCommand(
    Guid WorkspaceId,
    Guid LotId,
    Guid? TenantId,
    DateOnly? StartsOn,
    DateOnly? EndsOn,
    int RentDueDay,
    long RentExcludingChargesCents,
    long ChargesCents,
    long DepositCents,
    string PaymentTerms,
    string Notes) : IRequest<Result<LotOccupancyDto>>;
