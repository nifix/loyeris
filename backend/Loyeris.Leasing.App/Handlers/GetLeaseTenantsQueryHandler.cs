using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetLeaseTenantsQuery"/>.
/// </summary>
public class GetLeaseTenantsQueryHandler(ILeasingReadRepository repository)
    : IRequestHandler<GetLeaseTenantsQuery, Result<IReadOnlyList<LeaseTenantDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<LeaseTenantDto>>> Handle(GetLeaseTenantsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<LeaseTenantDto>>.Success(await repository.ListLeaseTenantsAsync(cancellationToken));
}
