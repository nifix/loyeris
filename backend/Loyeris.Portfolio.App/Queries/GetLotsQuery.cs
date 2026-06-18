using Loyeris.Portfolio.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Queries;

/// <summary>
/// Query that lists rental lots.
/// </summary>
public record GetLotsQuery() : IRequest<Result<IReadOnlyList<LotDto>>>;
