using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Api.Contracts.Portfolio;

/// <summary>
/// HTTP payload used to create or update a rental lot.
/// </summary>
public record SaveLotRequest(
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
    string Notes);
