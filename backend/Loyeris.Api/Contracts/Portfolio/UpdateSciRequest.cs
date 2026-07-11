using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Api.Contracts.Portfolio;

/// <summary>
/// HTTP payload used to update a SCI inside the current workspace.
/// </summary>
public record UpdateSciRequest(
    string Name,
    string Siren,
    TaxRegime TaxRegime,
    SciStatus Status,
    string Street,
    string PostalCode,
    string City,
    string Country,
    DateOnly? IncorporatedOn);
