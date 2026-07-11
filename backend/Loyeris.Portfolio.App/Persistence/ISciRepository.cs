using Loyeris.Portfolio.Core.Entities;

namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Provides write persistence for SCI aggregates.
/// </summary>
public interface ISciRepository
{
    /// <summary>
    /// Gets a tracked SCI inside the specified workspace boundary.
    /// </summary>
    Task<Sci> GetByIdAsync(Guid workspaceId, Guid sciId, CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new SCI and translates unique-index collisions into domain-neutral outcomes.
    /// </summary>
    Task<SciCreationPersistenceResult> CreateAsync(Sci sci, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes to an existing SCI and translates unique-index collisions.
    /// </summary>
    Task<SciUpdatePersistenceResult> UpdateAsync(Sci sci, CancellationToken cancellationToken);
}
