using Loyeris.Portfolio.App.Dtos;

namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Provides read-only projections for Portfolio screens and endpoints.
/// </summary>
public interface IPortfolioReadRepository
{
    /// <summary>
    /// Gets one SCI inside a workspace boundary.
    /// </summary>
    /// <param name="workspaceId">The workspace boundary used to isolate portfolio data.</param>
    /// <param name="sciId">The SCI identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The SCI when it belongs to the workspace; otherwise, <see langword="null"/>.</returns>
    Task<SciDto> GetSciAsync(Guid workspaceId, Guid sciId, CancellationToken cancellationToken);

    /// <summary>
    /// Lists SCI structures.
    /// </summary>
    /// <param name="workspaceId">The workspace boundary used to isolate portfolio data.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The SCI structures.</returns>
    Task<IReadOnlyList<SciDto>> ListScisAsync(Guid workspaceId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets one rental lot inside a workspace boundary.
    /// </summary>
    Task<LotDto> GetLotAsync(Guid workspaceId, Guid lotId, CancellationToken cancellationToken);

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
    Task<IReadOnlyList<LotDto>> ListLotsAsync(Guid workspaceId, CancellationToken cancellationToken);
}
