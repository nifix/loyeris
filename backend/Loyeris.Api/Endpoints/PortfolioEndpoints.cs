using Loyeris.Api.Contracts.Portfolio;
using Loyeris.Api.Extensions;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.IdentityAccess.Core.Enums;
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

        group.MapGet("/lots", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetLotsQuery())).ToHttpResult();
            })
            .WithName("GetPortfolioLots");
    }

    private static Guid? GetAuthenticatedUserId(HttpContext httpContext)
    {
        return Guid.TryParse(httpContext.User.FindFirst("sub")?.Value, out var userId)
            ? userId
            : null;
    }
}
