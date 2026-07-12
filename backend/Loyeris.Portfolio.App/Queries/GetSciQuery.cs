using Loyeris.Portfolio.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Queries;

/// <summary>
/// Query that returns one SCI from a workspace.
/// </summary>
public record GetSciQuery(Guid WorkspaceId, Guid SciId) : IRequest<Result<SciDto>>;
