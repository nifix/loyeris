namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Describes the database outcome of updating a SCI.
/// </summary>
public enum SciUpdatePersistenceResult
{
    Updated,
    DuplicateName,
    DuplicateSiren
}
