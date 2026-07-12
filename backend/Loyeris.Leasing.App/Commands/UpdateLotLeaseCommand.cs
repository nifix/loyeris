using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Commands;

/// <summary>
/// Updates the dates and financial terms of an existing lot lease.
/// </summary>
public record UpdateLotLeaseCommand(
    Guid WorkspaceId,
    Guid LotId,
    Guid LeaseId,
    DateOnly StartsOn,
    DateOnly? EndsOn,
    int RentDueDay,
    long RentExcludingChargesCents,
    long ChargesCents,
    long DepositCents,
    string PaymentTerms,
    string Notes) : IRequest<Result<LotOccupancyDto>>;
