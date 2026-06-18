using Loyeris.Api.Extensions;
using Loyeris.IdentityAccess.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Identity Access read endpoints.
/// </summary>
public static class IdentityAccessEndpoints
{
    /// <summary>
    /// Maps Identity Access GET routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterIdentityAccessEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/identity-access").WithTags("Identity Access");

        group.MapGet("/users", async (IMediator mediator) =>
                (await mediator.Send(new GetUsersQuery())).ToHttpResult())
            .WithName("GetIdentityAccessUsers");

        group.MapGet("/workspaces", async (IMediator mediator) =>
                (await mediator.Send(new GetWorkspacesQuery())).ToHttpResult())
            .WithName("GetIdentityAccessWorkspaces");

        group.MapGet("/workspace-members", async (IMediator mediator) =>
                (await mediator.Send(new GetWorkspaceMembersQuery())).ToHttpResult())
            .WithName("GetIdentityAccessWorkspaceMembers");

        group.MapGet("/auth-sessions", async (IMediator mediator) =>
                (await mediator.Send(new GetAuthSessionsQuery())).ToHttpResult())
            .WithName("GetIdentityAccessAuthSessions");

        group.MapGet("/auth-events", async (IMediator mediator) =>
                (await mediator.Send(new GetAuthEventsQuery())).ToHttpResult())
            .WithName("GetIdentityAccessAuthEvents");
    }
}
