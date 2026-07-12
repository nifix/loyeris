using Loyeris.Leasing.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Queries;

/// <summary>
/// Query that lists tenants available for assignment to a lot.
/// </summary>
public record GetAvailableTenantsQuery(
    Guid WorkspaceId,
    Guid? CurrentLotId) : IRequest<Result<IReadOnlyList<AvailableTenantDto>>>;
