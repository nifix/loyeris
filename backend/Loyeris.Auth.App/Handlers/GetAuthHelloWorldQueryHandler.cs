using Loyeris.Auth.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Auth.App.Handlers;

public class GetAuthHelloWorldQueryHandler() : IRequestHandler<GetAuthHelloWorldQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetAuthHelloWorldQuery request, CancellationToken cancellationToken)
    {
        // Placeholder logic, testing purposes only
        return await Task.FromResult(Result<string>.Created("Hello, World from auth created!"));
        return await Task.FromResult(Result<string>.Success("Hello, World from auth created!"));
        return await Task.FromResult(Result<string>.Failure(
            new Error("Auth.Unauthorized", "Unauthorized", ErrorType.Unauthorized))
        );
        return await Task.FromResult(Result<string>.NoContent());
        return await Task.FromResult(Result<string>.Failure(
            new Error("Auth.Conflict", "Conflict", ErrorType.Conflict))
        );
        return await Task.FromResult(Result<string>.Success("Hello, World from auth success!"));
    }
}
