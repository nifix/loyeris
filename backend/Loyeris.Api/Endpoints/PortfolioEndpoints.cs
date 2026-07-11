using Loyeris.Api.Extensions;
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

        group.MapGet("/scis", async (IMediator mediator) =>
                (await mediator.Send(new GetScisQuery())).ToHttpResult())
            .WithName("GetPortfolioScis");

        group.MapGet("/sci-associates", async (IMediator mediator) =>
                (await mediator.Send(new GetSciAssociatesQuery())).ToHttpResult())
            .WithName("GetPortfolioSciAssociates");

        group.MapGet("/lots", async (IMediator mediator) =>
                (await mediator.Send(new GetLotsQuery())).ToHttpResult())
            .WithName("GetPortfolioLots");
    }
}
