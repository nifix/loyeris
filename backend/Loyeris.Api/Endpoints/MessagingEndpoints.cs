using Loyeris.Api.Extensions;
using Loyeris.Messaging.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Messaging read endpoints.
/// </summary>
public static class MessagingEndpoints
{
    /// <summary>
    /// Maps Messaging GET routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterMessagingEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/messaging").WithTags("Messaging").RequireAuthorization();

        group.MapGet("/notification-preferences", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetNotificationPreferencesQuery())).ToHttpResult();
            })
            .WithName("GetMessagingNotificationPreferences");

        group.MapGet("/outbox-messages", async (IMediator mediator) =>
            {
                return (await mediator.Send(new GetOutboxMessagesQuery())).ToHttpResult();
            })
            .WithName("GetMessagingOutboxMessages");
    }
}
