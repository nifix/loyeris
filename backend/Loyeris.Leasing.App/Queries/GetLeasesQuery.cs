using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that lists leases.
/// </summary>
public record GetLeasesQuery() : IRequest<Result<IReadOnlyList<LeaseDto>>>;
