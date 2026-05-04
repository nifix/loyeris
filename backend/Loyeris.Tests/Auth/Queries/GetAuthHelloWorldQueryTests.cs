using FluentAssertions;
using Loyeris.Auth.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Tests.Auth.Queries;

/// <summary>
/// Ensures the query record adheres to the MediatR IRequest contract.
/// </summary>
public class GetAuthHelloWorldQueryTests
{
    /// <summary>
    /// Verifies the query is assignable to <see cref="IRequest{T}"/> with <see cref="Result{T}"/> of string.
    /// </summary>
    [Test]
    public void Query_ShouldImplement_IRequest_Of_Result_String()
    {
        // Arrange
        var query = new GetAuthHelloWorldQuery();

        // Act — no action needed, structural assertion only

        // Assert
        query.Should().BeAssignableTo<IRequest<Result<string>>>();
    }

    /// <summary>
    /// Verifies two identical query instances are structurally equal (record value equality).
    /// </summary>
    [Test]
    public void Query_Instances_ShouldBeStructurallyEqual()
    {
        // Arrange
        var query1 = new GetAuthHelloWorldQuery();
        var query2 = new GetAuthHelloWorldQuery();

        // Act — no action needed, structural assertion only

        // Assert
        query1.Should().Be(query2);
    }
}
