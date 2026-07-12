using Loyeris.Leasing.App.Commands;
using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Core.Enums;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Cancels a lease so downstream records can continue referencing its identifier.
/// </summary>
public class DeleteLotLeaseCommandHandler(
    ILotOccupancyRepository repository,
    TimeProvider timeProvider) : IRequestHandler<DeleteLotLeaseCommand, Result<LotOccupancyDto>>
{
    private static readonly Error LeaseNotFound = new(
        "leasing.lease.not_found",
        "Le bail demandé est introuvable.",
        ErrorType.NotFound);

    /// <inheritdoc />
    public async Task<Result<LotOccupancyDto>> Handle(
        DeleteLotLeaseCommand request,
        CancellationToken cancellationToken)
    {
        var lease = await repository.GetLeaseAsync(
            request.WorkspaceId,
            request.LotId,
            request.LeaseId,
            cancellationToken);
        
        if (lease is null)
            return Result<LotOccupancyDto>.Failure(LeaseNotFound);

        lease.Status = LeaseStatus.Canceled;
        lease.UpdatedAt = timeProvider.GetUtcNow();
        
        await repository.SaveChangesAsync(cancellationToken);
        return Result<LotOccupancyDto>.NoContent();
    }
}
