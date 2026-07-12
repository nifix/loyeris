using FluentAssertions;
using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Handlers;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Handlers;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Messaging.App.Dtos;
using Loyeris.Messaging.App.Handlers;
using Loyeris.Messaging.App.Persistence;
using Loyeris.Messaging.App.Queries;
using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Handlers;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.RentCollection.App.Dtos;
using Loyeris.RentCollection.App.Handlers;
using Loyeris.RentCollection.App.Persistence;
using Loyeris.RentCollection.App.Queries;
using Loyeris.TaxPreparation.App.Dtos;
using Loyeris.TaxPreparation.App.Handlers;
using Loyeris.TaxPreparation.App.Persistence;
using Loyeris.TaxPreparation.App.Queries;
using Moq;

namespace Loyeris.Tests.Application;

/// <summary>
/// Ensures starter read query handlers return successful results from their repositories.
/// </summary>
public class ReadQueryHandlerTests
{
    /// <summary>
    /// Verifies Identity Access read query handlers return successful results.
    /// </summary>
    [Test]
    public async Task IdentityAccessReadHandlers_ShouldReturnSuccessfulResults()
    {
        // Arrange
        var repository = new Mock<IIdentityAccessReadRepository>();
        repository.Setup(repo => repo.ListUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<UserDto>());
        repository.Setup(repo => repo.ListWorkspacesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<WorkspaceDto>());
        repository.Setup(repo => repo.ListWorkspaceMembersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<WorkspaceMemberDto>());
        repository.Setup(repo => repo.ListAuthSessionsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<AuthSessionDto>());
        repository.Setup(repo => repo.ListAuthEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<AuthEventDto>());

        // Act
        var users = await new GetUsersQueryHandler(repository.Object).Handle(new GetUsersQuery(), CancellationToken.None);
        var workspaces = await new GetWorkspacesQueryHandler(repository.Object).Handle(new GetWorkspacesQuery(), CancellationToken.None);
        var members = await new GetWorkspaceMembersQueryHandler(repository.Object).Handle(new GetWorkspaceMembersQuery(), CancellationToken.None);
        var sessions = await new GetAuthSessionsQueryHandler(repository.Object).Handle(new GetAuthSessionsQuery(), CancellationToken.None);
        var events = await new GetAuthEventsQueryHandler(repository.Object).Handle(new GetAuthEventsQuery(), CancellationToken.None);

        // Assert
        new[] { users.IsSuccess, workspaces.IsSuccess, members.IsSuccess, sessions.IsSuccess, events.IsSuccess }
            .Should()
            .OnlyContain(isSuccess => isSuccess);
    }

    /// <summary>
    /// Verifies Portfolio read query handlers return successful results.
    /// </summary>
    [Test]
    public async Task PortfolioReadHandlers_ShouldReturnSuccessfulResults()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var repository = new Mock<IPortfolioReadRepository>();
        repository.Setup(repo => repo.ListScisAsync(workspaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<SciDto>());
        
        var sciId = Guid.NewGuid();
        var sci = new SciDto(
            sciId,
            workspaceId,
            "SCI Test",
            null,
            TaxRegime.IR,
            SciStatus.Active,
            null,
            null,
            null,
            "FR",
            null,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            null);
        
        repository.Setup(repo => repo.GetSciAsync(workspaceId, sciId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sci);
        repository.Setup(repo => repo.ListSciAssociatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<SciAssociateDto>());
        repository.Setup(repo => repo.ListLotsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<LotDto>());

        // Act
        var scis = await new GetScisQueryHandler(repository.Object).Handle(
            new GetScisQuery(workspaceId),
            CancellationToken.None);
        
        var singleSci = await new GetSciQueryHandler(repository.Object).Handle(
            new GetSciQuery(workspaceId, sciId),
            CancellationToken.None);
        
        var associates = await new GetSciAssociatesQueryHandler(repository.Object).Handle(new GetSciAssociatesQuery(), CancellationToken.None);
        var lots = await new GetLotsQueryHandler(repository.Object).Handle(new GetLotsQuery(), CancellationToken.None);

        // Assert
        new[] { scis.IsSuccess, singleSci.IsSuccess, associates.IsSuccess, lots.IsSuccess }
            .Should()
            .OnlyContain(isSuccess => isSuccess);
        
        repository.Verify(repo => repo.ListScisAsync(workspaceId, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(repo => repo.GetSciAsync(workspaceId, sciId, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies Leasing read query handlers return successful results.
    /// </summary>
    [Test]
    public async Task LeasingReadHandlers_ShouldReturnSuccessfulResults()
    {
        // Arrange
        var repository = new Mock<ILeasingReadRepository>();
        repository.Setup(repo => repo.ListTenantsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<TenantDto>());
        repository.Setup(repo => repo.ListLeasesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<LeaseDto>());
        repository.Setup(repo => repo.ListLeaseTenantsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<LeaseTenantDto>());

        // Act
        var tenants = await new GetTenantsQueryHandler(repository.Object).Handle(new GetTenantsQuery(), CancellationToken.None);
        var leases = await new GetLeasesQueryHandler(repository.Object).Handle(new GetLeasesQuery(), CancellationToken.None);
        var leaseTenants = await new GetLeaseTenantsQueryHandler(repository.Object).Handle(new GetLeaseTenantsQuery(), CancellationToken.None);

        // Assert
        new[] { tenants.IsSuccess, leases.IsSuccess, leaseTenants.IsSuccess }
            .Should()
            .OnlyContain(isSuccess => isSuccess);
    }

    /// <summary>
    /// Verifies Rent Collection read query handlers return successful results.
    /// </summary>
    [Test]
    public async Task RentCollectionReadHandlers_ShouldReturnSuccessfulResults()
    {
        // Arrange
        var repository = new Mock<IRentCollectionReadRepository>();
        repository.Setup(repo => repo.ListDeadlinesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<RentDeadlineDto>());
        repository.Setup(repo => repo.ListPaymentsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<RentPaymentDto>());
        repository.Setup(repo => repo.ListRemindersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<RentReminderDto>());

        // Act
        var deadlines = await new GetRentDeadlinesQueryHandler(repository.Object).Handle(new GetRentDeadlinesQuery(), CancellationToken.None);
        var payments = await new GetRentPaymentsQueryHandler(repository.Object).Handle(new GetRentPaymentsQuery(), CancellationToken.None);
        var reminders = await new GetRentRemindersQueryHandler(repository.Object).Handle(new GetRentRemindersQuery(), CancellationToken.None);

        // Assert
        new[] { deadlines.IsSuccess, payments.IsSuccess, reminders.IsSuccess }
            .Should()
            .OnlyContain(isSuccess => isSuccess);
    }

    /// <summary>
    /// Verifies Tax Preparation read query handlers return successful results.
    /// </summary>
    [Test]
    public async Task TaxPreparationReadHandlers_ShouldReturnSuccessfulResults()
    {
        // Arrange
        var repository = new Mock<ITaxPreparationReadRepository>();
        repository.Setup(repo => repo.ListFiscalPeriodsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<FiscalPeriodDto>());
        repository.Setup(repo => repo.ListRentalExpensesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<RentalExpenseDto>());

        // Act
        var fiscalPeriods = await new GetFiscalPeriodsQueryHandler(repository.Object).Handle(new GetFiscalPeriodsQuery(), CancellationToken.None);
        var expenses = await new GetRentalExpensesQueryHandler(repository.Object).Handle(new GetRentalExpensesQuery(), CancellationToken.None);

        // Assert
        new[] { fiscalPeriods.IsSuccess, expenses.IsSuccess }
            .Should()
            .OnlyContain(isSuccess => isSuccess);
    }

    /// <summary>
    /// Verifies Messaging read query handlers return successful results.
    /// </summary>
    [Test]
    public async Task MessagingReadHandlers_ShouldReturnSuccessfulResults()
    {
        // Arrange
        var repository = new Mock<IMessagingReadRepository>();
        repository.Setup(repo => repo.ListNotificationPreferencesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<NotificationPreferenceDto>());
        repository.Setup(repo => repo.ListOutboxMessagesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<OutboxMessageDto>());

        // Act
        var preferences = await new GetNotificationPreferencesQueryHandler(repository.Object).Handle(new GetNotificationPreferencesQuery(), CancellationToken.None);
        var messages = await new GetOutboxMessagesQueryHandler(repository.Object).Handle(new GetOutboxMessagesQuery(), CancellationToken.None);

        // Assert
        new[] { preferences.IsSuccess, messages.IsSuccess }
            .Should()
            .OnlyContain(isSuccess => isSuccess);
    }
}
