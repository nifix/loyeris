using FluentAssertions;
using Loyeris.Leasing.App.Commands;
using Loyeris.Leasing.App.Handlers;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.Core.Entities;
using Loyeris.Leasing.Core.Enums;
using Loyeris.Shared.Results;
using Moq;

namespace Loyeris.Tests.Leasing.Handlers;

/// <summary>
/// Ensures lot occupancy commands maintain active leases and tenant availability.
/// </summary>
public class LotOccupancyCommandHandlerTests
{
    /// <summary>
    /// Verifies assigning an available tenant creates an active lease and primary link.
    /// </summary>
    [Test]
    public async Task SaveOccupancy_ShouldCreate_ActiveLeaseForAvailableTenant()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            FirstName = "Camille",
            LastName = "Robert",
            Status = TenantStatus.Active
        };
        
        Lease? persistedLease = null;
        
        var repository = new Mock<ILotOccupancyRepository>();
        repository.Setup(repo => repo.GetActiveTenantAsync(workspaceId, tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);
        repository.Setup(repo => repo.AddAsync(It.IsAny<Lease>(), It.IsAny<CancellationToken>()))
            .Callback<Lease, CancellationToken>((lease, _) => persistedLease = lease)
            .Returns(Task.CompletedTask);
        
        var handler = new SaveLotOccupancyCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var expectedEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));
        var command = CreateCommand(workspaceId, lotId, tenant.Id) with { EndsOn = expectedEndDate };
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        persistedLease.Should().NotBeNull();
        persistedLease!.Status.Should().Be(LeaseStatus.Active);
        persistedLease.LotId.Should().Be(lotId);
        persistedLease.EndsOn.Should().Be(expectedEndDate);
        result.Value.EndsOn.Should().Be(expectedEndDate);
        persistedLease.LeaseTenants.Should().ContainSingle(link =>
            link.TenantId == tenant.Id && link.Role == LeaseTenantRole.Primary);
        repository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies a tenant assigned to another current lease cannot be reused.
    /// </summary>
    [Test]
    public async Task SaveOccupancy_ShouldReturnConflict_WhenTenantIsAssignedElsewhere()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        
        var repository = new Mock<ILotOccupancyRepository>();
        repository.Setup(repo => repo.GetActiveTenantAsync(workspaceId, tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tenant { Id = tenantId, WorkspaceId = workspaceId, Status = TenantStatus.Active });
        repository.Setup(repo => repo.IsAssignedElsewhereAsync(
                tenantId,
                lotId,
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var handler = new SaveLotOccupancyCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(CreateCommand(workspaceId, lotId, tenantId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("leasing.occupancy.tenant_unavailable");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        repository.Verify(repo => repo.AddAsync(It.IsAny<Lease>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies clearing a lot ends its active lease without deleting its history.
    /// </summary>
    [Test]
    public async Task SaveOccupancy_ShouldEnd_ActiveLeaseWhenTenantIsCleared()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = Guid.NewGuid(),
            LotId = lotId,
            Status = LeaseStatus.Active,
            StartsOn = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3))
        };
        
        var repository = new Mock<ILotOccupancyRepository>();
        repository.Setup(repo => repo.GetActiveLeaseAsync(lotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lease);
        
        var handler = new SaveLotOccupancyCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(CreateCommand(workspaceId, lotId, null), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(SuccessType.NoContent);
        lease.Status.Should().Be(LeaseStatus.Ended);
        lease.EndsOn.Should().NotBeNull();
        repository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies an existing historical lease can be edited without changing its tenant.
    /// </summary>
    [Test]
    public async Task UpdateLease_ShouldUpdateTerms_WhenDatesDoNotOverlap()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            FirstName = "Camille",
            LastName = "Robert"
        };
        
        var lease = new Lease
        {
            Id = Guid.NewGuid(),
            LotId = lotId,
            Status = LeaseStatus.Ended,
            StartsOn = new DateOnly(2026, 1, 1),
            EndsOn = new DateOnly(2026, 6, 30),
            LeaseTenants =
            [
                new LeaseTenant
                {
                    TenantId = tenant.Id,
                    Tenant = tenant,
                    Role = LeaseTenantRole.Primary
                }
            ]
        };
        
        var repository = new Mock<ILotOccupancyRepository>();
        repository.Setup(candidate => candidate.GetLeaseAsync(
                workspaceId,
                lotId,
                lease.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(lease);
        
        var handler = new UpdateLotLeaseCommandHandler(repository.Object, TimeProvider.System);
        var command = new UpdateLotLeaseCommand(
            workspaceId,
            lotId,
            lease.Id,
            new DateOnly(2026, 1, 2),
            new DateOnly(2026, 6, 29),
            7,
            70000,
            6000,
            70000,
            "Virement",
            "Bail corrigé");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        lease.StartsOn.Should().Be(new DateOnly(2026, 1, 2));
        lease.EndsOn.Should().Be(new DateOnly(2026, 6, 29));
        lease.RentExcludingChargesCents.Should().Be(70000);
        result.Value.TenantId.Should().Be(tenant.Id);
        repository.Verify(candidate => candidate.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies an edit is rejected when its dates overlap another lease on the lot.
    /// </summary>
    [Test]
    public async Task UpdateLease_ShouldReturnConflict_WhenDatesOverlapAnotherLease()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var repository = new Mock<ILotOccupancyRepository>();
        repository.Setup(candidate => candidate.GetLeaseAsync(
                workspaceId,
                lotId,
                leaseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Lease { Id = leaseId, LotId = lotId });
        repository.Setup(candidate => candidate.HasOverlappingLeaseAsync(
                lotId,
                leaseId,
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new UpdateLotLeaseCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new UpdateLotLeaseCommand(
                workspaceId,
                lotId,
                leaseId,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 6, 30),
                5,
                65000,
                5000,
                65000,
                null,
                null),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("leasing.lease.overlap");
        result.Error.Type.Should().Be(ErrorType.Conflict);
        repository.Verify(candidate => candidate.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies deleting a lease cancels it without removing its persisted identity.
    /// </summary>
    [Test]
    public async Task DeleteLease_ShouldCancelLeaseAndPreserveHistory()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = Guid.NewGuid(),
            LotId = lotId,
            Status = LeaseStatus.Active
        };
        var repository = new Mock<ILotOccupancyRepository>();
        repository.Setup(candidate => candidate.GetLeaseAsync(
                workspaceId,
                lotId,
                lease.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(lease);
        var handler = new DeleteLotLeaseCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(
            new DeleteLotLeaseCommand(workspaceId, lotId, lease.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(SuccessType.NoContent);
        lease.Status.Should().Be(LeaseStatus.Canceled);
        repository.Verify(candidate => candidate.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static SaveLotOccupancyCommand CreateCommand(Guid workspaceId, Guid lotId, Guid? tenantId)
    {
        return new(
            workspaceId,
            lotId,
            tenantId,
            tenantId.HasValue ? new DateOnly(2026, 7, 1) : null,
            null,
            5,
            65000,
            5000,
            65000,
            "Virement mensuel",
            null);
    }
}
