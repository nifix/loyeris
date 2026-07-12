namespace Loyeris.Portfolio.App.Persistence;

/// <summary>
/// Describes the database outcome of saving a rental lot.
/// </summary>
public enum LotPersistenceResult
{
    Saved,
    DuplicateReference
}
