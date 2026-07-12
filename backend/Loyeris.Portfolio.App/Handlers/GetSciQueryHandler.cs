using Loyeris.Portfolio.App.Dtos;
using Loyeris.Portfolio.App.Persistence;
using Loyeris.Portfolio.App.Queries;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.Portfolio.App.Handlers;

/// <summary>
/// Handles <see cref="GetSciQuery"/>.
/// </summary>
public class GetSciQueryHandler(IPortfolioReadRepository repository)
    : IRequestHandler<GetSciQuery, Result<SciDto>>
{
    private static readonly Error NotFound = new(
        "portfolio.sci.not_found",
        "La SCI demandée est introuvable.",
        ErrorType.NotFound);

    /// <inheritdoc />
    public async Task<Result<SciDto>> Handle(GetSciQuery request, CancellationToken cancellationToken)
    {
        var sci = await repository.GetSciAsync(request.WorkspaceId, request.SciId, cancellationToken);
        return sci is null
            ? Result<SciDto>.Failure(NotFound)
            : Result<SciDto>.Success(sci);
    }
}
