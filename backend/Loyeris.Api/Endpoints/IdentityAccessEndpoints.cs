using Loyeris.Api.Contracts.IdentityAccess;
using Loyeris.Api.Extensions;
using Loyeris.IdentityAccess.App.Commands;
using Loyeris.IdentityAccess.App.Queries;
using MediatR;
using Loyeris.Shared.Configuration;
using Microsoft.Extensions.Options;

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

        group.MapPost("/auth/login", async (
                LoginRequest request,
                HttpContext httpContext,
                IMediator mediator,
                IOptions<JwtOptions> jwtOptions,
                CancellationToken cancellationToken) =>
            {
                var command = new LoginCommand(
                    request.Email,
                    request.Password,
                    request.RememberMe,
                    GetIpAddress(httpContext),
                    GetUserAgent(httpContext));
                
                var result = await mediator.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return result.ToHttpResult();

                SetRefreshCookie(httpContext, jwtOptions.Value, result.Value);
                return Results.Ok(ToAuthenticationResponse(result.Value));
            })
            .WithName("LoginIdentityAccessAccount")
            .RequireRateLimiting("identity-login");

        group.MapPost("/auth/refresh", async (
                HttpContext httpContext,
                IMediator mediator,
                IOptions<JwtOptions> jwtOptions,
                CancellationToken cancellationToken) =>
            {
                httpContext.Request.Cookies.TryGetValue(jwtOptions.Value.RefreshCookieName, out var refreshToken);
                var result = await mediator.Send(new RefreshSessionCommand(
                    refreshToken,
                    GetIpAddress(httpContext),
                    GetUserAgent(httpContext)), cancellationToken);

                if (!result.IsSuccess)
                {
                    DeleteRefreshCookie(httpContext, jwtOptions.Value);
                    return result.ToHttpResult();
                }

                SetRefreshCookie(httpContext, jwtOptions.Value, result.Value);
                return Results.Ok(ToAuthenticationResponse(result.Value));
            })
            .WithName("RefreshIdentityAccessSession")
            .RequireRateLimiting("identity-refresh");

        group.MapPost("/auth/logout", async (
                HttpContext httpContext,
                IMediator mediator,
                IOptions<JwtOptions> jwtOptions,
                CancellationToken cancellationToken) =>
            {
                httpContext.Request.Cookies.TryGetValue(jwtOptions.Value.RefreshCookieName, out var refreshToken);
                var result = await mediator.Send(new LogoutCommand(
                    refreshToken,
                    GetIpAddress(httpContext),
                    GetUserAgent(httpContext)), cancellationToken);
                
                DeleteRefreshCookie(httpContext, jwtOptions.Value);
                return result.ToHttpResult();
            })
            .WithName("LogoutIdentityAccessSession");

        group.MapGet("/users", async (IMediator mediator) =>
                (await mediator.Send(new GetUsersQuery())).ToHttpResult())
            .WithName("GetIdentityAccessUsers")
            .RequireAuthorization();

        group.MapGet("/workspaces", async (IMediator mediator) =>
                (await mediator.Send(new GetWorkspacesQuery())).ToHttpResult())
            .WithName("GetIdentityAccessWorkspaces")
            .RequireAuthorization();

        group.MapGet("/workspace-members", async (IMediator mediator) =>
                (await mediator.Send(new GetWorkspaceMembersQuery())).ToHttpResult())
            .WithName("GetIdentityAccessWorkspaceMembers")
            .RequireAuthorization();

        group.MapGet("/auth-sessions", async (IMediator mediator) =>
                (await mediator.Send(new GetAuthSessionsQuery())).ToHttpResult())
            .WithName("GetIdentityAccessAuthSessions")
            .RequireAuthorization();

        group.MapGet("/auth-events", async (IMediator mediator) =>
                (await mediator.Send(new GetAuthEventsQuery())).ToHttpResult())
            .WithName("GetIdentityAccessAuthEvents")
            .RequireAuthorization();
    }

    private static AuthenticationResponse ToAuthenticationResponse(Loyeris.IdentityAccess.App.Dtos.AuthenticationSessionDto session)
        => new(
            session.AccessToken,
            session.AccessTokenExpiresAt,
            session.UserId,
            session.Email,
            session.FirstName,
            session.LastName);

    private static void SetRefreshCookie(
        HttpContext httpContext,
        JwtOptions options,
        Loyeris.IdentityAccess.App.Dtos.AuthenticationSessionDto session)
    {
        var cookieOptions = CreateCookieOptions(httpContext);
        if (session.PersistentRefreshCookie)
            cookieOptions.Expires = session.RefreshTokenExpiresAt;

        httpContext.Response.Cookies.Append(options.RefreshCookieName, session.RefreshToken, cookieOptions);
    }

    private static void DeleteRefreshCookie(HttpContext httpContext, JwtOptions options)
        => httpContext.Response.Cookies.Delete(options.RefreshCookieName, CreateCookieOptions(httpContext));

    private static CookieOptions CreateCookieOptions(HttpContext httpContext)
        => new()
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/api/identity-access/auth"
        };

    private static string GetIpAddress(HttpContext httpContext)
        => httpContext.Connection.RemoteIpAddress?.ToString();

    private static string GetUserAgent(HttpContext httpContext)
        => httpContext.Request.Headers.UserAgent.ToString();
}
