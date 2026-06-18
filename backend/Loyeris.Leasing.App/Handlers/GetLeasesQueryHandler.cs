using Loyeris.Leasing.App.Dtos;
using Loyeris.Leasing.App.Persistence;
using Loyeris.Leasing.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Leasing.App.Handlers;

/// <summary>
/// Handles <see cref="GetLeasesQuery"/>.
/// </summary>
public class GetLeasesQueryHandler(ILeasingReadRepository repository)
    : IRequestHandler<GetLeasesQuery, Result<IReadOnlyList<LeaseDto>>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<LeaseDto>>> Handle(GetLeasesQuery request, CancellationToken cancellationToken)
        => Result<IReadOnlyList<LeaseDto>>.Success(await repository.ListLeasesAsync(cancellationToken));
}
