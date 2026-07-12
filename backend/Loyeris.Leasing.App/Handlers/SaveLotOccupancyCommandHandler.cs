using Loyeris.Leasing.App.Commands;
using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Maintains the active lease and primary tenant attached to a lot.
/// </summary>
public class SaveLotOccupancyCommandHandler(
    ILotOccupancyRepository repository,
    TimeProvider timeProvider) : IRequestHandler<SaveLotOccupancyCommand, Result<LotOccupancyDto>>
{
    private static readonly Error InvalidOccupancy = new(
        "leasing.occupancy.invalid",
        "Les informations de location sont invalides.",
        ErrorType.Validation);

    private static readonly Error TenantUnavailable = new(
        "leasing.occupancy.tenant_unavailable",
        "Ce locataire est déjà affecté à un autre lot ou n'est plus disponible.",
        ErrorType.Conflict);

    /// <inheritdoc />
    public async Task<Result<LotOccupancyDto>> Handle(
        SaveLotOccupancyCommand request,
        CancellationToken cancellationToken)
    {
        if (request.WorkspaceId == Guid.Empty || request.LotId == Guid.Empty)
            return Result<LotOccupancyDto>.Failure(InvalidOccupancy);

        var now = timeProvider.GetUtcNow();
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var activeLease = await repository.GetActiveLeaseAsync(request.LotId, cancellationToken);

        if (!request.TenantId.HasValue)
        {
            if (activeLease is null) 
                return Result<LotOccupancyDto>.NoContent();
            
            activeLease.Status = LeaseStatus.Ended;
            activeLease.EndsOn = today < activeLease.StartsOn ? activeLease.StartsOn : today;
            activeLease.UpdatedAt = now;
            await repository.SaveChangesAsync(cancellationToken);

            return Result<LotOccupancyDto>.NoContent();
        }

        if (!IsValidLease(request, today))
            return Result<LotOccupancyDto>.Failure(InvalidOccupancy);

        var tenant = await repository.GetActiveTenantAsync(
            request.WorkspaceId,
            request.TenantId.Value,
            cancellationToken);
        if (tenant is null || await repository.IsAssignedElsewhereAsync(
                tenant.Id,
                request.LotId,
                today,
                cancellationToken))
            return Result<LotOccupancyDto>.Failure(TenantUnavailable);

        var currentPrimary = activeLease?.LeaseTenants
            .FirstOrDefault(link => link.Role == LeaseTenantRole.Primary);

        if (activeLease is not null && currentPrimary?.TenantId == tenant.Id)
        {
            ApplyTerms(activeLease, request, now);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<LotOccupancyDto>.Success(ToDto(activeLease, tenant));
        }

        if (activeLease is not null)
        {
            if (request.StartsOn <= activeLease.StartsOn)
                return Result<LotOccupancyDto>.Failure(InvalidOccupancy);

            activeLease.Status = LeaseStatus.Ended;
            if (request.StartsOn != null) 
                activeLease.EndsOn = request.StartsOn.Value.AddDays(-1);
            
            activeLease.UpdatedAt = now;
        }

        var lease = CreateLease(request, tenant, now);
        await repository.AddAsync(lease, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<LotOccupancyDto>.Success(ToDto(lease, tenant));
    }

    private static bool IsValidLease(SaveLotOccupancyCommand request, DateOnly currentDate)
    {
        return request.StartsOn.HasValue
               && (!request.EndsOn.HasValue || request.EndsOn >= request.StartsOn)
               && (!request.EndsOn.HasValue || request.EndsOn >= currentDate)
               && request.RentDueDay is >= 1 and <= 28
               && request.RentExcludingChargesCents >= 0
               && request.ChargesCents >= 0
               && request.DepositCents >= 0;
    }

    private static void ApplyTerms(
        Lease lease,
        SaveLotOccupancyCommand request,
        DateTimeOffset now)
    {
        lease.StartsOn = request.StartsOn!.Value;
        lease.EndsOn = request.EndsOn;
        lease.RentDueDay = request.RentDueDay;
        lease.RentExcludingChargesCents = request.RentExcludingChargesCents;
        lease.ChargesCents = request.ChargesCents;
        lease.DepositCents = request.DepositCents;
        lease.PaymentTerms = NormalizeOptional(request.PaymentTerms);
        lease.Notes = NormalizeOptional(request.Notes);
        lease.UpdatedAt = now;
    }

    private static Lease CreateLease(
        SaveLotOccupancyCommand request,
        Tenant tenant,
        DateTimeOffset now)
    {
        var lease = new Lease
        {
            Id = Guid.NewGuid(),
            LotId = request.LotId,
            Status = LeaseStatus.Active,
            StartsOn = request.StartsOn!.Value,
            EndsOn = request.EndsOn,
            RentDueDay = request.RentDueDay,
            RentExcludingChargesCents = request.RentExcludingChargesCents,
            ChargesCents = request.ChargesCents,
            DepositCents = request.DepositCents,
            PaymentTerms = NormalizeOptional(request.PaymentTerms),
            Notes = NormalizeOptional(request.Notes),
            CreatedAt = now,
            UpdatedAt = now
        };
        lease.LeaseTenants.Add(new LeaseTenant
        {
            Id = Guid.NewGuid(),
            LeaseId = lease.Id,
            Lease = lease,
            TenantId = tenant.Id,
            Tenant = tenant,
            Role = LeaseTenantRole.Primary,
            CreatedAt = now,
            UpdatedAt = now
        });
        return lease;
    }

    private static LotOccupancyDto ToDto(Lease lease, Tenant tenant)
    {
        return new(
            lease.LotId,
            lease.Id,
            tenant.Id,
            tenant.FirstName,
            tenant.LastName,
            lease.StartsOn,
            lease.EndsOn,
            lease.RentDueDay,
            lease.RentExcludingChargesCents,
            lease.ChargesCents,
            lease.DepositCents,
            lease.PaymentTerms,
            lease.Notes);
    }

    private static string NormalizeOptional(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
