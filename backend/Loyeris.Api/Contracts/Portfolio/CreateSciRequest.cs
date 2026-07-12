using Loyeris.Portfolio.Core.Enums;

namespace Loyeris.Api.Contracts.Portfolio;

/// <summary>
/// HTTP payload used to create a SCI inside the current workspace.
/// </summary>
public record CreateSciRequest(
    string Name,
    string Siren,
    TaxRegime TaxRegime,
    SciStatus Status,
    string Street,
    string PostalCode,
    string City,
    string Country,
    DateOnly? IncorporatedOn);
