using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Portfolio.App.Dtos;

/// <summary>
/// Read model returned when listing SCI structures.
/// </summary>
public record SciDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string Siren,
    TaxRegime TaxRegime,
    SciStatus Status,
    string Street,
    string PostalCode,
    string City,
    string Country,
    DateOnly? IncorporatedOn,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ArchivedAt);
