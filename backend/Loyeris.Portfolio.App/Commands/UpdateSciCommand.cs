using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Commands;

/// <summary>
/// Updates a SCI structure inside an authenticated workspace.
/// </summary>
public record UpdateSciCommand(
    Guid WorkspaceId,
    Guid SciId,
    string Name,
    string Siren,
    TaxRegime TaxRegime,
    SciStatus Status,
    string Street,
    string PostalCode,
    string City,
    string Country,
    DateOnly? IncorporatedOn) : IRequest<Result<SciDto>>;
