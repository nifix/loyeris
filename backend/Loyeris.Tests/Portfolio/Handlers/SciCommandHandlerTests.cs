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
/// Ensures SCI commands validate legal data and remain scoped to their workspace.
/// </summary>
public class SciCommandHandlerTests
{
    /// <summary>
    /// Verifies <see cref="CreateSciCommandHandler"/> normalizes and persists a valid SCI.
    /// </summary>
    [Test]
    public async Task CreateSci_ShouldPersist_NormalizedWorkspaceScopedEntity()
    {
        // Arrange
        Sci? persistedSci = null;
        var repository = new Mock<ISciRepository>();
        repository.Setup(repo => repo.CreateAsync(It.IsAny<Sci>(), It.IsAny<CancellationToken>()))
            .Callback<Sci, CancellationToken>((sci, _) => persistedSci = sci)
            .ReturnsAsync(SciCreationPersistenceResult.Created);
        var handler = new CreateSciCommandHandler(repository.Object, TimeProvider.System);
        var workspaceId = Guid.NewGuid();

        // Act
        var result = await handler.Handle(new CreateSciCommand(
            workspaceId,
            "  SCI Les Tilleuls  ",
            "123 456 789",
            TaxRegime.IR,
            SciStatus.Active,
            " 12 rue des Tilleuls ",
            "69000",
            " Lyon ",
            "fr",
            new DateOnly(2024, 1, 10)), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(SuccessType.Created);
        persistedSci.Should().NotBeNull();
        persistedSci!.WorkspaceId.Should().Be(workspaceId);
        persistedSci.Name.Should().Be("SCI Les Tilleuls");
        persistedSci.Siren.Should().Be("123456789");
        persistedSci.Street.Should().Be("12 rue des Tilleuls");
        persistedSci.City.Should().Be("Lyon");
        persistedSci.Country.Should().Be("FR");
        persistedSci.ArchivedAt.Should().BeNull();
    }

    /// <summary>
    /// Verifies malformed legal data is rejected before persistence.
    /// </summary>
    [TestCase("", "123456789")]
    [TestCase("SCI Test", "123")]
    [TestCase("SCI Test", "ABCDEFGHI")]
    public async Task CreateSci_ShouldReject_InvalidIdentity(string name, string siren)
    {
        // Arrange
        var repository = new Mock<ISciRepository>();
        var handler = new CreateSciCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(CreateCommand(name, siren), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("portfolio.sci.invalid");
        repository.Verify(repo => repo.CreateAsync(It.IsAny<Sci>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies future incorporation dates are rejected.
    /// </summary>
    [Test]
    public async Task CreateSci_ShouldReject_FutureIncorporationDate()
    {
        // Arrange
        var repository = new Mock<ISciRepository>();
        var handler = new CreateSciCommandHandler(repository.Object, TimeProvider.System);
        var command = CreateCommand("SCI Test", null) with
        {
            IncorporatedOn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("portfolio.sci.invalid");
    }

    /// <summary>
    /// Verifies unique database collisions are exposed through stable conflict codes.
    /// </summary>
    [TestCase(SciCreationPersistenceResult.DuplicateName, "portfolio.sci.name_already_exists")]
    [TestCase(SciCreationPersistenceResult.DuplicateSiren, "portfolio.sci.siren_already_exists")]
    public async Task CreateSci_ShouldTranslate_UniqueIndexCollisions(
        SciCreationPersistenceResult persistenceResult,
        string expectedCode)
    {
        // Arrange
        var repository = new Mock<ISciRepository>();
        repository.Setup(repo => repo.CreateAsync(It.IsAny<Sci>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceResult);
        var handler = new CreateSciCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(CreateCommand("SCI Test", "123456789"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(expectedCode);
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    /// <summary>
    /// Verifies <see cref="UpdateSciCommandHandler"/> updates the tracked SCI and preserves its creation data.
    /// </summary>
    [Test]
    public async Task UpdateSci_ShouldPersist_NormalizedWorkspaceScopedEntity()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var sciId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddYears(-1);
        var existingSci = new Sci
        {
            Id = sciId,
            WorkspaceId = workspaceId,
            Name = "SCI Initiale",
            Country = "FR",
            Status = SciStatus.Active,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
        
        var repository = new Mock<ISciRepository>();
        repository.Setup(repo => repo.GetByIdAsync(workspaceId, sciId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSci);
        repository.Setup(repo => repo.UpdateAsync(existingSci, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SciUpdatePersistenceResult.Updated);
        var handler = new UpdateSciCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(new UpdateSciCommand(
            workspaceId,
            sciId,
            "  SCI Modifiée  ",
            "123 456 789",
            TaxRegime.IR,
            SciStatus.Archived,
            " 8 rue Carnot ",
            "69000",
            " Lyon ",
            "fr",
            new DateOnly(2020, 5, 12)), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.SuccessType.Should().Be(SuccessType.Ok);
        existingSci.Name.Should().Be("SCI Modifiée");
        existingSci.Siren.Should().Be("123456789");
        existingSci.Street.Should().Be("8 rue Carnot");
        existingSci.City.Should().Be("Lyon");
        existingSci.Country.Should().Be("FR");
        existingSci.Status.Should().Be(SciStatus.Archived);
        existingSci.ArchivedAt.Should().NotBeNull();
        existingSci.CreatedAt.Should().Be(createdAt);
        existingSci.UpdatedAt.Should().BeAfter(createdAt);
    }

    /// <summary>
    /// Verifies an update cannot reach a SCI from another workspace.
    /// </summary>
    [Test]
    public async Task UpdateSci_ShouldReturnNotFound_WhenSciIsOutsideWorkspace()
    {
        // Arrange
        var repository = new Mock<ISciRepository>();
        var handler = new UpdateSciCommandHandler(repository.Object, TimeProvider.System);
        var command = CreateUpdateCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("portfolio.sci.not_found");
        result.Error.Type.Should().Be(ErrorType.NotFound);
        repository.Verify(repo => repo.UpdateAsync(It.IsAny<Sci>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifies update-time unique collisions keep the same stable API error codes as creation.
    /// </summary>
    [TestCase(SciUpdatePersistenceResult.DuplicateName, "portfolio.sci.name_already_exists")]
    [TestCase(SciUpdatePersistenceResult.DuplicateSiren, "portfolio.sci.siren_already_exists")]
    public async Task UpdateSci_ShouldTranslate_UniqueIndexCollisions(
        SciUpdatePersistenceResult persistenceResult,
        string expectedCode)
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var sciId = Guid.NewGuid();
        var repository = new Mock<ISciRepository>();
        repository.Setup(repo => repo.GetByIdAsync(workspaceId, sciId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Sci { Id = sciId, WorkspaceId = workspaceId });
        repository.Setup(repo => repo.UpdateAsync(It.IsAny<Sci>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistenceResult);
        var handler = new UpdateSciCommandHandler(repository.Object, TimeProvider.System);

        // Act
        var result = await handler.Handle(CreateUpdateCommand(workspaceId, sciId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(expectedCode);
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    private static CreateSciCommand CreateCommand(string name, string? siren)
    {
        return new CreateSciCommand(
            Guid.NewGuid(),
            name,
            siren,
            TaxRegime.IR,
            SciStatus.Active,
            null,
            null,
            null,
            "FR",
            null);
    }

    private static UpdateSciCommand CreateUpdateCommand(Guid workspaceId, Guid sciId)
    {
        return new UpdateSciCommand(
            workspaceId,
            sciId,
            "SCI Test",
            "123456789",
            TaxRegime.IR,
            SciStatus.Active,
            null,
            null,
            null,
            "FR",
            null);
    }
}
