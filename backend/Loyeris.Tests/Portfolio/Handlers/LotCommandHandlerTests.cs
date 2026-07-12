using FluentAssertions;
using Loyeris.Portfolio.App.Commands;
using Loyeris.Portfolio.App.Handlers;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.Core.Entities;
using Loyeris.Portfolio.Core.Enums;
using Loyeris.Shared.Results;
using Moq;

namespace Loyeris.Tests.Portfolio.Handlers;

/// <summary>
/// Ensures rental lot commands validate and persist workspace-scoped data.
/// </summary>
public class LotCommandHandlerTests
{
    /// <summary>
    /// Verifies <see cref="CreateLotCommandHandler"/> normalizes and persists a complete lot.
    /// </summary>
    [Test]
    public async Task CreateLot_ShouldPersist_NormalizedLot()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var sci = new Sci { Id = Guid.NewGuid(), WorkspaceId = workspaceId, Name = "SCI Test" };
        Lot? persistedLot = null;
        
        var repository = new Mock<ILotRepository>();
        repository.Setup(repo => repo.GetSciByIdAsync(workspaceId, sci.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sci);
        repository.Setup(repo => repo.CreateAsync(It.IsAny<Lot>(), It.IsAny<CancellationToken>()))
            .Callback<Lot, CancellationToken>((lot, _) => persistedLot = lot)
            .ReturnsAsync(LotPersistenceResult.Saved);
        
        var handler = new CreateLotCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var command = CreateCommand(workspaceId, sci.Id) with { Type = LotType.T5 };
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(SuccessType.Created);
        persistedLot.Should().NotBeNull();
        persistedLot!.Reference.Should().Be("Lot A01");
        persistedLot.Street.Should().Be("12 rue des Tilleuls");
        persistedLot.Country.Should().Be("FR");
        persistedLot.Type.Should().Be(LotType.T5);
        persistedLot.PotentialRentExcludingChargesCents.Should().Be(65000);
    }

    /// <summary>
    /// Verifies duplicate references are translated to a stable conflict.
    /// </summary>
    [Test]
    public async Task CreateLot_ShouldReturnConflict_WhenReferenceExistsInSci()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var sci = new Sci { Id = Guid.NewGuid(), WorkspaceId = workspaceId, Name = "SCI Test" };
        
        var repository = new Mock<ILotRepository>();
        repository.Setup(repo => repo.GetSciByIdAsync(workspaceId, sci.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sci);
        repository.Setup(repo => repo.CreateAsync(It.IsAny<Lot>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(LotPersistenceResult.DuplicateReference);
        
        var handler = new CreateLotCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(CreateCommand(workspaceId, sci.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("portfolio.lot.reference_already_exists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    /// <summary>
    /// Verifies lot updates cannot cross the workspace boundary.
    /// </summary>
    [Test]
    public async Task UpdateLot_ShouldReturnNotFound_WhenLotIsOutsideWorkspace()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var lotId = Guid.NewGuid();
        var repository = new Mock<ILotRepository>();
        var handler = new UpdateLotCommandHandler(repository.Object, TimeProvider.System);
        var create = CreateCommand(workspaceId, Guid.NewGuid());
        var command = new UpdateLotCommand(
            workspaceId,
            lotId,
            create.SciId,
            create.Reference,
            create.Type,
            create.Status,
            create.Street,
            create.PostalCode,
            create.City,
            create.Country,
            create.SurfaceSqm,
            create.PotentialRentExcludingChargesCents,
            create.PotentialChargesCents,
            create.SuggestedDepositCents,
            create.Notes);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("portfolio.lot.not_found");
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<Lot>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies <see cref="UpdateLotCommandHandler"/> updates an existing workspace lot.
    /// </summary>
    [Test]
    public async Task UpdateLot_ShouldPersist_ExistingLot()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var sci = new Sci { Id = Guid.NewGuid(), WorkspaceId = workspaceId, Name = "SCI Test" };
        var lot = new Lot
        {
            Id = Guid.NewGuid(),
            SciId = sci.Id,
            Sci = sci,
            Reference = "Lot initial",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-10)
        };
        
        var repository = new Mock<ILotRepository>();
        repository.Setup(repo => repo.GetByIdAsync(workspaceId, lot.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lot);
        repository.Setup(repo => repo.UpdateAsync(lot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(LotPersistenceResult.Saved);
        
        var create = CreateCommand(workspaceId, sci.Id);
        var command = new UpdateLotCommand(
            workspaceId,
            lot.Id,
            sci.Id,
            "Lot rénové",
            create.Type,
            LotStatus.Archived,
            create.Street,
            create.PostalCode,
            create.City,
            create.Country,
            create.SurfaceSqm,
            create.PotentialRentExcludingChargesCents,
            create.PotentialChargesCents,
            create.SuggestedDepositCents,
            create.Notes);
        
        var handler = new UpdateLotCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        lot.Reference.Should().Be("Lot rénové");
        lot.Status.Should().Be(LotStatus.Archived);
        lot.ArchivedAt.Should().NotBeNull();
        repository.Verify(repo => repo.UpdateAsync(lot, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateLotCommand CreateCommand(Guid workspaceId, Guid sciId)
    {
        return new(
            workspaceId,
            sciId,
            "  Lot A01  ",
            LotType.T2,
            LotStatus.Active,
            " 12 rue des Tilleuls ",
            "69000",
            " Lyon ",
            "fr",
            42.5m,
            65000,
            5000,
            65000,
            " Notes ");
    }
}
