using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetTenantsQuery"/>.
/// </summary>
public class GetTenantsQueryHandler(ILeasingReadRepository repository)
    : IRequestHandler<GetTenantsQuery, Result<IReadOnlyList<TenantDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<TenantDto>>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<TenantDto>>.Success(await repository.ListTenantsAsync(cancellationToken));
}
