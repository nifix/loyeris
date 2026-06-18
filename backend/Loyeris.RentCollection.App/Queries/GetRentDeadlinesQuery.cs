using Loyeris.RentCollection.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.RentCollection.App.Queries;

/// <summary>
/// Query that lists rent deadlines.
/// </summary>
public record GetRentDeadlinesQuery() : IRequest<Result<IReadOnlyList<RentDeadlineDto>>>;
