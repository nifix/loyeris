using Loyeris.Portfolio.App.Commands;
using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.Core.Entities;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Validates and creates rental lots within a workspace boundary.
/// </summary>
public class CreateLotCommandHandler(
    ILotRepository repository,
    TimeProvider timeProvider) : IRequestHandler<CreateLotCommand, Result<LotDto>>
{
    private static readonly Error InvalidLot = new(
        "portfolio.lot.invalid",
        "Les informations du lot sont invalides.",
        ErrorType.Validation);

    private static readonly Error SciNotFound = new(
        "portfolio.lot.sci_not_found",
        "La SCI sélectionnée est introuvable.",
        ErrorType.NotFound);

    private static readonly Error DuplicateReference = new(
        "portfolio.lot.reference_already_exists",
        "Cette référence est déjà utilisée dans la SCI sélectionnée.",
        ErrorType.Conflict);

    /// <inheritdoc />
    public async Task<Result<LotDto>> Handle(CreateLotCommand request, CancellationToken cancellationToken)
    {
        var reference = request.Reference?.Trim();
        var street = request.Street?.Trim();
        var postalCode = request.PostalCode?.Trim();
        var city = request.City?.Trim();
        var country = request.Country?.Trim().ToUpperInvariant();
        var notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

        if (!IsValid(request, reference, street, postalCode, city, country))
            return Result<LotDto>.Failure(InvalidLot);

        var sci = await repository.GetSciByIdAsync(request.WorkspaceId, request.SciId, cancellationToken);
        if (sci is null)
            return Result<LotDto>.Failure(SciNotFound);

        var now = timeProvider.GetUtcNow();
        var lot = new Lot
        {
            Id = Guid.NewGuid(),
            SciId = sci.Id,
            Sci = sci,
            Reference = reference,
            Type = request.Type,
            Status = request.Status,
            Street = street,
            PostalCode = postalCode,
            City = city,
            Country = country,
            SurfaceSqm = request.SurfaceSqm,
            PotentialRentExcludingChargesCents = request.PotentialRentExcludingChargesCents,
            PotentialChargesCents = request.PotentialChargesCents,
            SuggestedDepositCents = request.SuggestedDepositCents,
            Notes = notes,
            CreatedAt = now,
            UpdatedAt = now,
            ArchivedAt = request.Status == LotStatus.Archived ? now : null
        };

        var persistenceResult = await repository.CreateAsync(lot, cancellationToken);
        return persistenceResult == LotPersistenceResult.DuplicateReference
            ? Result<LotDto>.Failure(DuplicateReference)
            : Result<LotDto>.Created(ToDto(lot));
    }

    private static bool IsValid(
        CreateLotCommand request,
        string reference,
        string street,
        string postalCode,
        string city,
        string country)
    {
        return request.WorkspaceId != Guid.Empty
               && request.SciId != Guid.Empty
               && !string.IsNullOrWhiteSpace(reference) && reference.Length <= 80
               && Enum.IsDefined(request.Type)
               && Enum.IsDefined(request.Status)
               && !string.IsNullOrWhiteSpace(street) && street.Length <= 180
               && !string.IsNullOrWhiteSpace(postalCode) && postalCode.Length <= 20
               && !string.IsNullOrWhiteSpace(city) && city.Length <= 120
               && country is { Length: 2 } && country.All(char.IsLetter)
               && (request.SurfaceSqm is null or > 0 and <= 99999.99m)
               && request.PotentialRentExcludingChargesCents >= 0
               && request.PotentialChargesCents >= 0
               && request.SuggestedDepositCents >= 0;
    }

    private static LotDto ToDto(Lot lot)
    {
        return new LotDto(
            lot.Id,
            lot.SciId,
            lot.Sci.Name,
            lot.Reference,
            lot.Type,
            lot.Status,
            lot.Street,
            lot.PostalCode,
            lot.City,
            lot.Country,
            lot.SurfaceSqm,
            lot.PotentialRentExcludingChargesCents,
            lot.PotentialChargesCents,
            lot.SuggestedDepositCents,
            lot.Notes,
            lot.CreatedAt,
            lot.UpdatedAt,
            lot.ArchivedAt);
    }
}
