using Loyeris.Portfolio.Core.Entities;

namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Provides write persistence for rental lots.
/// </summary>
public interface ILotRepository
{
    /// <summary>
    /// Gets a tracked lot inside a workspace boundary.
    /// </summary>
    Task<Lot> GetByIdAsync(Guid workspaceId, Guid lotId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the SCI that will own a lot when it belongs to the workspace.
    /// </summary>
    Task<Sci> GetSciByIdAsync(Guid workspaceId, Guid sciId, CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new lot.
    /// </summary>
    Task<LotPersistenceResult> CreateAsync(Lot lot, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes to an existing lot.
    /// </summary>
    Task<LotPersistenceResult> UpdateAsync(Lot lot, CancellationToken cancellationToken);
}
