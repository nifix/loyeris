using Loyeris.Api.Contracts.Leasing;
using Loyeris.Api.Extensions;
using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Leasing.App.Commands;
using Loyeris.Leasing.App.Queries;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Leasing read endpoints.
/// </summary>
public static class LeasingEndpoints
{
    /// <summary>
    /// Maps Leasing GET routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterLeasingEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/leasing").WithTags("Leasing").RequireAuthorization();

        group.MapGet("/tenants", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetTenantsQuery())).ToHttpResult();
            })
            .WithName("GetLeasingTenants");

        group.MapGet("/leases", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetLeasesQuery())).ToHttpResult();
            })
            .WithName("GetLeasingLeases");

        group.MapGet("/lease-tenants", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetLeaseTenantsQuery())).ToHttpResult();
            })
            .WithName("GetLeasingLeaseTenants");

        group.MapGet("/available-tenants", async (
                Guid? lotId,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetAvailableTenantsQuery(workspaceAccess.Value.WorkspaceId, lotId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetAvailableLeasingTenants");

        group.MapGet("/lot-occupancies", async (
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetLotOccupanciesQuery(workspaceAccess.Value.WorkspaceId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetLeasingLotOccupancies");

        group.MapGet("/lots/{lotId:guid}/occupancy", async (
                Guid lotId,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetLotOccupancyQuery(workspaceAccess.Value.WorkspaceId, lotId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetLeasingLotOccupancy");

        group.MapGet("/lots/{lotId:guid}/leases", async (
                Guid lotId,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetLotLeaseHistoryQuery(workspaceAccess.Value.WorkspaceId, lotId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetLeasingLotLeaseHistory");

        group.MapPut("/lots/{lotId:guid}/occupancy", async (
                Guid lotId,
                SaveLotOccupancyRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();
                
                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                    return Results.Forbid();

                var lot = await mediator.Send(
                    new GetLotQuery(workspaceAccess.Value.WorkspaceId, lotId),
                    cancellationToken);
                
                if (!lot.IsSuccess)
                    return lot.ToHttpResult();
                
                if (request.TenantId.HasValue && lot.Value.Status == Loyeris.Portfolio.Core.Enums.LotStatus.Archived)
                {
                    const string errorCode = "leasing.occupancy.archived_lot";
                    return Results.Problem(
                        title: errorCode,
                        detail: "Un lot archivé ne peut pas recevoir de locataire.",
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        extensions: new Dictionary<string, object> { ["errorCode"] = errorCode });
                }

                var command = new SaveLotOccupancyCommand(
                    workspaceAccess.Value.WorkspaceId,
                    lotId,
                    request.TenantId,
                    request.StartsOn,
                    request.EndsOn,
                    request.RentDueDay,
                    request.RentExcludingChargesCents,
                    request.ChargesCents,
                    request.DepositCents,
                    request.PaymentTerms,
                    request.Notes);
                
                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("SaveLeasingLotOccupancy");

        group.MapPut("/lots/{lotId:guid}/leases/{leaseId:guid}", async (
                Guid lotId,
                Guid leaseId,
                UpdateLotLeaseRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();
                
                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                    return Results.Forbid();

                var command = new UpdateLotLeaseCommand(
                    workspaceAccess.Value.WorkspaceId,
                    lotId,
                    leaseId,
                    request.StartsOn,
                    request.EndsOn,
                    request.RentDueDay,
                    request.RentExcludingChargesCents,
                    request.ChargesCents,
                    request.DepositCents,
                    request.PaymentTerms,
                    request.Notes);
                
                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("UpdateLeasingLotLease");

        group.MapDelete("/lots/{lotId:guid}/leases/{leaseId:guid}", async (
                Guid lotId,
                Guid leaseId,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var workspaceAccess = await GetWorkspaceAccessAsync(httpContext, mediator, cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();
                
                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                    return Results.Forbid();

                return (await mediator.Send(
                    new DeleteLotLeaseCommand(
                        workspaceAccess.Value.WorkspaceId,
                        lotId,
                        leaseId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("DeleteLeasingLotLease");
    }

    private static async Task<Result<WorkspaceAccessDto>> GetWorkspaceAccessAsync(
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(httpContext.User.FindFirst("sub")?.Value, out var userId))
        {
            return Result<WorkspaceAccessDto>.Failure(
                new Error(
                    "identity.unauthorized",
                    "La session est invalide.",
                    ErrorType.Unauthorized));
        }

        return await mediator.Send(new GetPrimaryWorkspaceAccessQuery(userId), cancellationToken);
    }
}
