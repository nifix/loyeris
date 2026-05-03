using Loyeris.Api.Extensions;
using Loyeris.Auth.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

public static class Auth
{
    /// <summary>
    /// Registers the authentication endpoint group and maps related routes to the specified route builder.
    /// </summary>
    /// <param name="routes">The route builder used to define and register endpoint routes for the application.</param>
    /// <param name="mediator">mediatR DI</param>
    public static void RegisterAuthEndpointGroup(this IEndpointRouteBuilder routes, IMediator mediator)
    {
        var sampleEndpointGroup = routes.MapGroup("api/auth");
        
        // Auth hello world
        sampleEndpointGroup
            .MapGet("/", async () => (await mediator.Send(new GetAuthHelloWorldQuery())).ToHttpResult())
            .WithName("GetAuthHelloWorld");
    }
}