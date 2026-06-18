using Loyeris.Portfolio.App.Dtos;

namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Provides read-only projections for Portfolio screens and endpoints.
/// </summary>
public interface IPortfolioReadRepository
{
    /// <summary>
    /// Lists SCI structures.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The SCI structures.</returns>
    Task<IReadOnlyList<SciDto>> ListScisAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists SCI associates.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The SCI associates.</returns>
    Task<IReadOnlyList<SciAssociateDto>> ListSciAssociatesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lists rental lots.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The rental lots.</returns>
    Task<IReadOnlyList<LotDto>> ListLotsAsync(CancellationToken cancellationToken);
}
