using FluentAssertions;
using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Handlers;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Moq;

namespace Loyeris.Tests.Leasing.Handlers;

/// <summary>
/// Tests the monthly period selected by <see cref="GetLotOccupanciesQueryHandler"/>.
/// </summary>
public class GetLotOccupanciesQueryHandlerTests
{
    /// <summary>
    /// Ensures the query includes every lease overlapping the current calendar month.
    /// </summary>
    [Test]
    public async Task Handle_ShouldRequestTheCompleteCurrentMonth()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var repository = new Mock<ILeasingReadRepository>();
        repository
            .Setup(candidate => candidate.ListLotOccupanciesAsync(
                workspaceId,
                new DateOnly(2026, 7, 1),
                new DateOnly(2026, 7, 31),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<LotOccupancyDto>());
        
        var timeProvider = new Mock<TimeProvider>();
        timeProvider
            .Setup(candidate => candidate.GetUtcNow())
            .Returns(new DateTimeOffset(2026, 7, 12, 10, 0, 0, TimeSpan.Zero));
        
        var handler = new GetLotOccupanciesQueryHandler(repository.Object, timeProvider.Object);

        // Act
        var result = await handler.Handle(
            new GetLotOccupanciesQuery(workspaceId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repository.Verify(candidate => candidate.ListLotOccupanciesAsync(
            workspaceId,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 31),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
