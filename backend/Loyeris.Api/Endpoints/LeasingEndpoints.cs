using Loyeris.Api.Extensions;
using Loyeris.Leasing.App.Queries;
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
                (await mediator.Send(new GetTenantsQuery())).ToHttpResult())
            .WithName("GetLeasingTenants");

        group.MapGet("/leases", async (IMediator mediator) =>
                (await mediator.Send(new GetLeasesQuery())).ToHttpResult())
            .WithName("GetLeasingLeases");

        group.MapGet("/lease-tenants", async (IMediator mediator) =>
                (await mediator.Send(new GetLeaseTenantsQuery())).ToHttpResult())
            .WithName("GetLeasingLeaseTenants");
    }
}
