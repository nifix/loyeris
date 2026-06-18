using Loyeris.Portfolio.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Queries;

/// <summary>
/// Query that lists SCI structures.
/// </summary>
public record GetScisQuery() : IRequest<Result<IReadOnlyList<SciDto>>>;
