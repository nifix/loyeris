namespace Loyeris.IdentityAccess.App.Persistence;

/// <summary>
/// Describes the persistence outcome of an atomic account registration.
/// </summary>
public enum AccountRegistrationPersistenceResult
{
    Created = 0,
    DuplicateEmail = 1
}
