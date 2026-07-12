using Loyeris.Portfolio.App.Commands;
using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.Core.Entities;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Validates and updates SCI structures within a workspace boundary.
/// </summary>
public class UpdateSciCommandHandler(
    ISciRepository repository,
    TimeProvider timeProvider) : IRequestHandler<UpdateSciCommand, Result<SciDto>>
{
    private static readonly Error InvalidSci = new(
        "portfolio.sci.invalid",
        "Les informations de la SCI sont invalides.",
        ErrorType.Validation);

    private static readonly Error NotFound = new(
        "portfolio.sci.not_found",
        "La SCI demandée est introuvable.",
        ErrorType.NotFound);

    private static readonly Error DuplicateName = new(
        "portfolio.sci.name_already_exists",
        "Une SCI de cet espace utilise déjà ce nom.",
        ErrorType.Conflict);

    private static readonly Error DuplicateSiren = new(
        "portfolio.sci.siren_already_exists",
        "Une SCI utilise déjà ce numéro SIREN.",
        ErrorType.Conflict);

    /// <inheritdoc />
    public async Task<Result<SciDto>> Handle(UpdateSciCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();
        var siren = RemoveWhitespace(request.Siren);
        var street = NormalizeOptional(request.Street);
        var postalCode = NormalizeOptional(request.PostalCode);
        var city = NormalizeOptional(request.City);
        var country = string.IsNullOrWhiteSpace(request.Country) ? "FR" : request.Country.Trim().ToUpperInvariant();
        var now = timeProvider.GetUtcNow();
        var today = DateOnly.FromDateTime(now.UtcDateTime);

        if (request.WorkspaceId == Guid.Empty
            || request.SciId == Guid.Empty
            || string.IsNullOrWhiteSpace(name)
            || name.Length > 180
            || siren is not null && (siren.Length != 9 || !siren.All(char.IsDigit))
            || street?.Length > 180
            || postalCode?.Length > 20
            || city?.Length > 120
            || country.Length != 2 || !country.All(char.IsLetter)
            || !Enum.IsDefined(request.TaxRegime)
            || !Enum.IsDefined(request.Status)
            || request.IncorporatedOn > today)
        {
            return Result<SciDto>.Failure(InvalidSci);
        }

        var sci = await repository.GetByIdAsync(request.WorkspaceId, request.SciId, cancellationToken);
        if (sci is null)
            return Result<SciDto>.Failure(NotFound);

        sci.Name = name;
        sci.Siren = siren;
        sci.TaxRegime = request.TaxRegime;
        sci.Status = request.Status;
        sci.Street = street;
        sci.PostalCode = postalCode;
        sci.City = city;
        sci.Country = country;
        sci.IncorporatedOn = request.IncorporatedOn;
        sci.UpdatedAt = now;
        sci.ArchivedAt = request.Status == SciStatus.Archived ? sci.ArchivedAt ?? now : null;

        var persistenceResult = await repository.UpdateAsync(sci, cancellationToken);
        if (persistenceResult == SciUpdatePersistenceResult.DuplicateName)
            return Result<SciDto>.Failure(DuplicateName);
        if (persistenceResult == SciUpdatePersistenceResult.DuplicateSiren)
            return Result<SciDto>.Failure(DuplicateSiren);

        return Result<SciDto>.Success(ToDto(sci));
    }

    private static SciDto ToDto(Sci sci)
    {
        return new(
            sci.Id,
            sci.WorkspaceId,
            sci.Name,
            sci.Siren,
            sci.TaxRegime,
            sci.Status,
            sci.Street,
            sci.PostalCode,
            sci.City,
            sci.Country,
            sci.IncorporatedOn,
            sci.CreatedAt,
            sci.UpdatedAt,
            sci.ArchivedAt);
    }

    private static string NormalizeOptional(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string RemoveWhitespace(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : string.Concat(value.Where(character => !char.IsWhiteSpace(character)));
    }
}
