using FluentAssertions;
using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.IdentityAccess.App.Handlers;
using Loyeris.IdentityAccess.App.Persistence;
using Loyeris.IdentityAccess.App.Queries;
using Loyeris.IdentityAccess.Core.Enums;
using Moq;

namespace Loyeris.Tests.IdentityAccess.Handlers;

/// <summary>
/// Ensures authenticated users are resolved to an active workspace boundary.
/// </summary>
public class WorkspaceAccessQueryHandlerTests
{
    /// <summary>
    /// Verifies the primary workspace access returned by persistence is exposed to API orchestration.
    /// </summary>
    [Test]
    public async Task GetPrimaryWorkspaceAccess_ShouldReturn_AvailableWorkspace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var access = new WorkspaceAccessDto(Guid.NewGuid(), WorkspaceRole.Owner);
        var repository = new Mock<IIdentityAccessReadRepository>();
        repository.Setup(repo => repo.GetPrimaryWorkspaceAccessAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(access);
        var handler = new GetPrimaryWorkspaceAccessQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(
            new GetPrimaryWorkspaceAccessQuery(userId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(access);
    }

    /// <summary>
    /// Verifies users without an active workspace receive a stable forbidden result.
    /// </summary>
    [Test]
    public async Task GetPrimaryWorkspaceAccess_ShouldReject_UserWithoutWorkspace()
    {
        // Arrange
        var repository = new Mock<IIdentityAccessReadRepository>();
        repository.Setup(repo => repo.GetPrimaryWorkspaceAccessAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkspaceAccessDto)null!);
        var handler = new GetPrimaryWorkspaceAccessQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(
            new GetPrimaryWorkspaceAccessQuery(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("identity.workspace_unavailable");
    }
}
