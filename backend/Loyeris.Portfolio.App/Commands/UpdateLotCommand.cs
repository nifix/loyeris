using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Commands;

/// <summary>
/// Updates a rental lot inside an authenticated workspace.
/// </summary>
public record UpdateLotCommand(
    Guid WorkspaceId,
    Guid LotId,
    Guid SciId,
    string Reference,
    LotType Type,
    LotStatus Status,
    string Street,
    string PostalCode,
    string City,
    string Country,
    decimal? SurfaceSqm,
    long PotentialRentExcludingChargesCents,
    long PotentialChargesCents,
    long SuggestedDepositCents,
    string Notes) : IRequest<Result<LotDto>>;
