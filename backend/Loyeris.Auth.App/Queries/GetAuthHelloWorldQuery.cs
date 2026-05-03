using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Auth.App.Queries;

public record GetAuthHelloWorldQuery() : IRequest<Result<string>>;