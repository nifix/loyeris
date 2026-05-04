using Loyeris.Auth.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Auth.App.Handlers;

/// <summary>
/// Handles the processing of the <see cref="GetAuthHelloWorldQuery"/> request.
/// </summary>
/// <remarks>
/// This handler returns a <see cref="Result{T}"/> of type <see cref="string"/>
/// containing a "Hello World" message. The result encapsulates different states,
/// such as success, failure, created, no content, and conflict.
/// </remarks>
/// <example>
/// The handler processes the query to generate an operation result that could
/// represent various outcomes, e.g., successful response, unauthorized access,
/// conflict in request, or no content available.
/// </example>
public class GetAuthHelloWorldQueryHandler() : IRequestHandler<GetAuthHelloWorldQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetAuthHelloWorldQuery request, CancellationToken cancellationToken)
    {
        // Placeholder logic, testing purposes only
        return await Task.FromResult(Result<string>.Created("Hello, World from auth created!"));
    }
}
