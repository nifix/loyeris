using Loyeris.Portfolio.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Queries;

/// <summary>
/// Query that lists SCI associates.
/// </summary>
public record GetSciAssociatesQuery() : IRequest<Result<IReadOnlyList<SciAssociateDto>>>;
