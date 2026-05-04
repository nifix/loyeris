using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Auth.App.Queries;

/// <summary>
/// A mediator query that represents a request to retrieve a "Hello World" message
/// with authentication context.
/// </summary>
/// <remarks>
/// This query is processed by an appropriate handler to generate a "Hello World"
/// response. It returns a <see cref="Result{T}"/> of type <see cref="string"/>
/// encapsulating the operation result.
/// </remarks>
public record GetAuthHelloWorldQuery() : IRequest<Result<string>>;