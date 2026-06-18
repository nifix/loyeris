using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that lists tenants.
/// </summary>
public record GetTenantsQuery() : IRequest<Result<IReadOnlyList<TenantDto>>>;
