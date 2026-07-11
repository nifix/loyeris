using Loyeris.Api.Contracts.IdentityAccess;
using Loyeris.Api.Extensions;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Identity Access endpoints.
/// </summary>
public static class IdentityAccessEndpoints
{
    /// <summary>
    /// Maps Identity Access read and public account routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterIdentityAccessEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/identity-access").WithTags("Identity Access");

        group.MapPost("/accounts", async (
                RegisterAccountRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new RegisterAccountCommand(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Password,
                    request.TermsAccepted,
                    httpContext.Connection.RemoteIpAddress?.ToString(),
                    httpContext.Request.Headers.UserAgent.ToString());

                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("RegisterIdentityAccessAccount")
            .RequireRateLimiting("identity-registration");

        group.MapPost("/email-verifications", async (
                VerifyEmailRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new VerifyEmailCommand(
                    request.Token,
                    httpContext.Connection.RemoteIpAddress?.ToString(),
                    httpContext.Request.Headers.UserAgent.ToString());

                return (await mediator.Send(command, cancellationToken)).ToHttpResult();
            })
            .WithName("VerifyIdentityAccessEmail")
            .RequireRateLimiting("identity-email-verification");

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
