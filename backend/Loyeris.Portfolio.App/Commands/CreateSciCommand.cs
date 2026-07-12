using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Commands;

/// <summary>
/// Creates a legal SCI structure inside an authenticated workspace.
/// </summary>
public record CreateSciCommand(
    Guid WorkspaceId,
    string Name,
    string Siren,
    TaxRegime TaxRegime,
    SciStatus Status,
    string Street,
    string PostalCode,
    string City,
    string Country,
    DateOnly? IncorporatedOn) : IRequest<Result<SciDto>>;
