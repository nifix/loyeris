using Loyeris.Leasing.App.Commands;
using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Updates an existing lease without rewriting its tenant history.
/// </summary>
public class UpdateLotLeaseCommandHandler(
    ILotOccupancyRepository repository,
    TimeProvider timeProvider) : IRequestHandler<UpdateLotLeaseCommand, Result<LotOccupancyDto>>
{
    private static readonly Error InvalidLease = new(
        "leasing.lease.invalid",
        "Les informations du bail sont invalides.",
        ErrorType.Validation);

    private static readonly Error LeaseNotFound = new(
        "leasing.lease.not_found",
        "Le bail demandé est introuvable.",
        ErrorType.NotFound);

    private static readonly Error OverlappingLease = new(
        "leasing.lease.overlap",
        "Les dates du bail chevauchent un autre bail du lot.",
        ErrorType.Conflict);

    /// <inheritdoc />
    public async Task<Result<LotOccupancyDto>> Handle(
        UpdateLotLeaseCommand request,
        CancellationToken cancellationToken)
    {
        if (!IsValid(request))
            return Result<LotOccupancyDto>.Failure(InvalidLease);

        var lease = await repository.GetLeaseAsync(
            request.WorkspaceId,
            request.LotId,
            request.LeaseId,
            cancellationToken);
        if (lease is null)
            return Result<LotOccupancyDto>.Failure(LeaseNotFound);

        if (await repository.HasOverlappingLeaseAsync(
                request.LotId,
                request.LeaseId,
                request.StartsOn,
                request.EndsOn,
                cancellationToken))
            return Result<LotOccupancyDto>.Failure(OverlappingLease);

        lease.StartsOn = request.StartsOn;
        lease.EndsOn = request.EndsOn;
        lease.RentDueDay = request.RentDueDay;
        lease.RentExcludingChargesCents = request.RentExcludingChargesCents;
        lease.ChargesCents = request.ChargesCents;
        lease.DepositCents = request.DepositCents;
        lease.PaymentTerms = NormalizeOptional(request.PaymentTerms);
        lease.Notes = NormalizeOptional(request.Notes);
        lease.UpdatedAt = timeProvider.GetUtcNow();
        await repository.SaveChangesAsync(cancellationToken);

        var primaryTenant = lease.LeaseTenants.Single(link => link.Role == LeaseTenantRole.Primary).Tenant;
        return Result<LotOccupancyDto>.Success(new LotOccupancyDto(
            lease.LotId,
            lease.Id,
            primaryTenant.Id,
            primaryTenant.FirstName,
            primaryTenant.LastName,
            lease.StartsOn,
            lease.EndsOn,
            lease.RentDueDay,
            lease.RentExcludingChargesCents,
            lease.ChargesCents,
            lease.DepositCents,
            lease.PaymentTerms,
            lease.Notes));
    }

    private static bool IsValid(UpdateLotLeaseCommand request)
    {
        return request.WorkspaceId != Guid.Empty
               && request.LotId != Guid.Empty
               && request.LeaseId != Guid.Empty
               && (!request.EndsOn.HasValue || request.EndsOn >= request.StartsOn)
               && request.RentDueDay is >= 1 and <= 28
               && request.RentExcludingChargesCents >= 0
               && request.ChargesCents >= 0
               && request.DepositCents >= 0;
    }

    private static string NormalizeOptional(string value) 
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
