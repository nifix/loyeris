namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Describes the database outcome of creating a new SCI.
/// </summary>
public enum SciCreationPersistenceResult
{
    Created,
    DuplicateName,
    DuplicateSiren
}
