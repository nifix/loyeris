using FluentAssertions;
using Loyeris.Auth.App.Handlers;
using Loyeris.Auth.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Tests.Auth.Handlers;

/// <summary>
/// Ensures the handler processes <see cref="GetAuthHelloWorldQuery"/> and returns the expected <see cref="Result{T}"/>.
/// </summary>
public class GetAuthHelloWorldQueryHandlerTests
{
    private readonly GetAuthHelloWorldQueryHandler _handler = new();

    /// <summary>
    /// Verifies the handler returns a successful result.
    /// </summary>
    [Test]
    public async Task Handle_ShouldReturnSuccess()
    {
        // Arrange
        var query = new GetAuthHelloWorldQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// Verifies the success type is <see cref="SuccessType.Created"/>.
    /// </summary>
    [Test]
    public async Task Handle_ShouldReturnCreatedSuccessType()
    {
        // Arrange
        var query = new GetAuthHelloWorldQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.SuccessType.Should().Be(SuccessType.Created);
    }

    /// <summary>
    /// Verifies the returned message matches the expected value.
    /// </summary>
    [Test]
    public async Task Handle_ShouldReturnExpectedMessage()
    {
        // Arrange
        var query = new GetAuthHelloWorldQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().Be("Hello, World from auth created!");
    }

    /// <summary>
    /// Verifies the result contains no error when the handler succeeds.
    /// </summary>
    [Test]
    public async Task Handle_ShouldNotHaveError_WhenSuccess()
    {
        // Arrange
        var query = new GetAuthHelloWorldQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Error.Should().BeNull();
    }

    /// <summary>
    /// Verifies the handler is assignable to <see cref="IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    [Test]
    public void Handler_ShouldImplement_IRequestHandler()
    {
        // Arrange — handler is already instantiated in the field initializer

        // Act — no action needed, structural assertion only

        // Assert
        _handler.Should().BeAssignableTo<IRequestHandler<GetAuthHelloWorldQuery, Result<string>>>();
    }
}
