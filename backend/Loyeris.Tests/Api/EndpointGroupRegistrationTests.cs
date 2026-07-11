using FluentAssertions;
using Loyeris.Api.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Loyeris.Tests.Api;

/// <summary>
/// Ensures Minimal API endpoint groups expose the expected starter read routes.
/// </summary>
public class EndpointGroupRegistrationTests
{
    /// <summary>
    /// Verifies every domain read endpoint route is registered.
    /// </summary>
    [Test]
    public void EndpointGroups_ShouldRegister_DomainReadRoutes()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Mock.Of<IMediator>());
        var app = builder.Build();

        // Act
        app.RegisterIdentityAccessEndpointGroup();
        app.RegisterPortfolioEndpointGroup();
        app.RegisterLeasingEndpointGroup();
        app.RegisterRentCollectionEndpointGroup();
        app.RegisterTaxPreparationEndpointGroup();
        app.RegisterMessagingEndpointGroup();

        var routes = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .ToArray();

        // Assert
        routes.Should().Contain([
            "api/identity-access/accounts",
            "api/identity-access/email-verifications",
            "api/identity-access/password-reset-requests",
            "api/identity-access/password-reset-validations",
            "api/identity-access/password-resets",
            "api/identity-access/auth/login",
            "api/identity-access/auth/refresh",
            "api/identity-access/auth/logout",
            "api/identity-access/users",
            "api/identity-access/workspaces",
            "api/identity-access/workspace-members",
            "api/identity-access/auth-sessions",
            "api/identity-access/auth-events",
            "api/portfolio/scis",
            "api/portfolio/sci-associates",
            "api/portfolio/lots",
            "api/leasing/tenants",
            "api/leasing/leases",
            "api/leasing/lease-tenants",
            "api/rent-collection/deadlines",
            "api/rent-collection/payments",
            "api/rent-collection/reminders",
            "api/tax-preparation/fiscal-periods",
            "api/tax-preparation/rental-expenses",
            "api/messaging/notification-preferences",
            "api/messaging/outbox-messages"
        ]);
    }
}
