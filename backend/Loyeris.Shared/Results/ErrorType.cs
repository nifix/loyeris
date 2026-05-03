namespace Loyeris.Shared.Results;

/// <summary>
/// Specifies the type of error that occurred.
/// </summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Failure
}