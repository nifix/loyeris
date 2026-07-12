using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Handles <see cref="GetLotQuery"/>.
/// </summary>
public class GetLotQueryHandler(IPortfolioReadRepository repository)
    : IRequestHandler<GetLotQuery, Result<LotDto>>
{
    private static readonly Error NotFound = new(
        "portfolio.lot.not_found",
        "Le lot demandé est introuvable.",
        ErrorType.NotFound);

    /// <inheritdoc />
    public async Task<Result<LotDto>> Handle(GetLotQuery request, CancellationToken cancellationToken)
    {
        var lot = await repository.GetLotAsync(request.WorkspaceId, request.LotId, cancellationToken);
        return lot is null
            ? Result<LotDto>.Failure(NotFound)
            : Result<LotDto>.Success(lot);
    }
}
