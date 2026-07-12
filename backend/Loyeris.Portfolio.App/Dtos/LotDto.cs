using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Portfolio.App.Dtos;

/// <summary>
/// Read model returned when listing rental lots.
/// </summary>
public record LotDto(
    Guid Id,
    Guid SciId,
    string SciName,
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
    string Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ArchivedAt);
