using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that lists lease tenant links.
/// </summary>
public record GetLeaseTenantsQuery() : IRequest<Result<IReadOnlyList<LeaseTenantDto>>>;
