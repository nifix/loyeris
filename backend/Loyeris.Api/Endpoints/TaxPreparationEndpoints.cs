using Loyeris.Api.Extensions;
using Loyeris.TaxPreparation.App.Queries;
using MediatR;

namespace Loyeris.Api.Endpoints;

/// <summary>
/// Registers Tax Preparation read endpoints.
/// </summary>
public static class TaxPreparationEndpoints
{
    /// <summary>
    /// Maps Tax Preparation GET routes.
    /// </summary>
    /// <param name="routes">The route builder used to define endpoint routes.</param>
    public static void RegisterTaxPreparationEndpointGroup(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/tax-preparation").WithTags("Tax Preparation");

        group.MapGet("/fiscal-periods", async (IMediator mediator) =>
                (await mediator.Send(new GetFiscalPeriodsQuery())).ToHttpResult())
            .WithName("GetTaxPreparationFiscalPeriods");

        group.MapGet("/rental-expenses", async (IMediator mediator) =>
                (await mediator.Send(new GetRentalExpensesQuery())).ToHttpResult())
            .WithName("GetTaxPreparationRentalExpenses");
    }
}
