using Loyeris.Api.Extensions;
using Loyeris.RentCollection.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Rent Collection read endpoints.
/// </summary>
public static class RentCollectionEndpoints
{
    /// <summary>
    /// Maps Rent Collection GET routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterRentCollectionEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/rent-collection").WithTags("Rent Collection").RequireAuthorization();

        group.MapGet("/deadlines", async (IMediator mediator) =>
                (await mediator.Send(new GetRentDeadlinesQuery())).ToHttpResult())
            .WithName("GetRentCollectionDeadlines");

        group.MapGet("/payments", async (IMediator mediator) =>
                (await mediator.Send(new GetRentPaymentsQuery())).ToHttpResult())
            .WithName("GetRentCollectionPayments");

        group.MapGet("/reminders", async (IMediator mediator) =>
                (await mediator.Send(new GetRentRemindersQuery())).ToHttpResult())
            .WithName("GetRentCollectionReminders");
    }
}
