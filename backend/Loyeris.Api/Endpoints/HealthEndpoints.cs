using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers unauthenticated operational health endpoints.
/// </summary>
public static class HealthEndpoints
{
    /// <summary>
    /// Maps process liveness and database readiness routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterHealthEndpointGroup(this IEndpointRouteBuilder routes)
    {
        routes.MapHealthChecks("/api/health/live", CreateOptions("live"))
            .AllowAnonymous()
            .WithName("GetLoyerisLiveness");

        routes.MapHealthChecks("/api/health/ready", CreateOptions("ready"))
            .AllowAnonymous()
            .WithName("GetLoyerisReadiness");
    }

    private static HealthCheckOptions CreateOptions(string tag)
    {
        return new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(tag),
            ResponseWriter = WriteResponseAsync
        };
    }

    private static Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        return JsonSerializer.SerializeAsync(
            context.Response.Body,
            new
            {
                status = report.Status.ToString(),
                checks = report.Entries.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.Status.ToString())
            },
            cancellationToken: context.RequestAborted);
    }
}
