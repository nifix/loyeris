using Loyeris.Api.Contracts.Portfolio;
using Loyeris.Api.Extensions;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.IdentityAccess.Core.Enums;
using Loyeris.Leasing.App.Queries;
using Loyeris.Portfolio.App.Commands;
using Loyeris.Portfolio.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Portfolio read endpoints.
/// </summary>
public static class PortfolioEndpoints
{
    /// <summary>
    /// Maps Portfolio GET routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterPortfolioEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/portfolio").WithTags("Portfolio").RequireAuthorization();

        group.MapGet("/scis", async (
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetScisQuery(workspaceAccess.Value.WorkspaceId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetPortfolioScis");

        group.MapGet("/scis/{sciId:guid}", async (
                Guid sciId,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetSciQuery(workspaceAccess.Value.WorkspaceId, sciId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetPortfolioSci");

        group.MapPost("/scis", async (
                CreateSciRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                {
                    const string errorCode = "portfolio.sci.create_forbidden";
                    return Results.Problem(
                        title: errorCode,
                        detail: "Vous ne disposez pas des droits nécessaires pour ajouter une SCI.",
                        statusCode: StatusCodes.Status403Forbidden,
                        extensions: new Dictionary<string, object> { ["errorCode"] = errorCode });
                }

                var command = new CreateSciCommand(
                    workspaceAccess.Value.WorkspaceId,
                    request.Name,
                    request.Siren,
                    request.TaxRegime,
                    request.Status,
                    request.Street,
                    request.PostalCode,
                    request.City,
                    request.Country,
                    request.IncorporatedOn);

                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("CreatePortfolioSci");

        group.MapPut("/scis/{sciId:guid}", async (
                Guid sciId,
                UpdateSciRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                {
                    const string errorCode = "portfolio.sci.update_forbidden";
                    return Results.Problem(
                        title: errorCode,
                        detail: "Vous ne disposez pas des droits nécessaires pour modifier cette SCI.",
                        statusCode: StatusCodes.Status403Forbidden,
                        extensions: new Dictionary<string, object> { ["errorCode"] = errorCode });
                }

                var command = new UpdateSciCommand(
                    workspaceAccess.Value.WorkspaceId,
                    sciId,
                    request.Name,
                    request.Siren,
                    request.TaxRegime,
                    request.Status,
                    request.Street,
                    request.PostalCode,
                    request.City,
                    request.Country,
                    request.IncorporatedOn);

                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("UpdatePortfolioSci");

        group.MapGet("/sci-associates", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetSciAssociatesQuery())).ToHttpResult();
            })
            .WithName("GetPortfolioSciAssociates");

        group.MapGet("/lots", async (
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetLotsQuery(workspaceAccess.Value.WorkspaceId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetPortfolioLots");

        group.MapGet("/lots/{lotId:guid}", async (
                Guid lotId,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();

                return (await mediator.Send(
                    new GetLotQuery(workspaceAccess.Value.WorkspaceId, lotId),
                    cancellationToken)).ToHttpResult();
            })
            .WithName("GetPortfolioLot");

        group.MapPost("/lots", async (
                SaveLotRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();
                
                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                    return LotWriteForbidden("ajouter");

                var command = new CreateLotCommand(
                    workspaceAccess.Value.WorkspaceId,
                    request.SciId,
                    request.Reference,
                    request.Type,
                    request.Status,
                    request.Street,
                    request.PostalCode,
                    request.City,
                    request.Country,
                    request.SurfaceSqm,
                    request.PotentialRentExcludingChargesCents,
                    request.PotentialChargesCents,
                    request.SuggestedDepositCents,
                    request.Notes);
                
                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("CreatePortfolioLot");

        group.MapPut("/lots/{lotId:guid}", async (
                Guid lotId,
                SaveLotRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userId = GetAuthenticatedUserId(httpContext);
                if (!userId.HasValue)
                    return Results.Unauthorized();

                var workspaceAccess = await mediator.Send(
                    new GetPrimaryWorkspaceAccessQuery(userId.Value),
                    cancellationToken);
                if (!workspaceAccess.IsSuccess)
                    return workspaceAccess.ToHttpResult();
                
                if (workspaceAccess.Value.Role == WorkspaceRole.Member)
                    return LotWriteForbidden("modifier");

                if (request.Status == Loyeris.Portfolio.Core.Enums.LotStatus.Archived)
                {
                    var occupancy = await mediator.Send(
                        new GetLotOccupancyQuery(workspaceAccess.Value.WorkspaceId, lotId),
                        cancellationToken);
                    if (occupancy.IsSuccess && occupancy.Value is not null)
                    {
                        const string errorCode = "portfolio.lot.active_occupancy";
                        return Results.Problem(
                            title: errorCode,
                            detail: "Retirez le locataire avant d'archiver ce lot.",
                            statusCode: StatusCodes.Status422UnprocessableEntity,
                            extensions: new Dictionary<string, object> { ["errorCode"] = errorCode });
                    }
                }

                var command = new UpdateLotCommand(
                    workspaceAccess.Value.WorkspaceId,
                    lotId,
                    request.SciId,
                    request.Reference,
                    request.Type,
                    request.Status,
                    request.Street,
                    request.PostalCode,
                    request.City,
                    request.Country,
                    request.SurfaceSqm,
                    request.PotentialRentExcludingChargesCents,
                    request.PotentialChargesCents,
                    request.SuggestedDepositCents,
                    request.Notes);
                
                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("UpdatePortfolioLot");
    }

    private static Guid? GetAuthenticatedUserId(HttpContext httpContext)
    {
        return Guid.TryParse(httpContext.User.FindFirst("sub")?.Value, out var userId)
            ? userId
            : null;
    }

    private static IResult LotWriteForbidden(string action)
    {
        const string errorCode = "portfolio.lot.write_forbidden";
        return Results.Problem(
            title: errorCode,
            detail: $"Vous ne disposez pas des droits nécessaires pour {action} un lot.",
            statusCode: StatusCodes.Status403Forbidden,
            extensions: new Dictionary<string, object> { ["errorCode"] = errorCode });
    }
}
