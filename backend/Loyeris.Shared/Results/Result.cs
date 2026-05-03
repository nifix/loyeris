namespace Loyeris.Shared.Results;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned when the operation succeeds.</typeparam>
public record Result<T>
{
    public bool IsSuccess { get; private init; }
    public T Value { get; private init; }
    public Error Error { get; private init; }
    public SuccessType SuccessType { get; init; } = SuccessType.None;

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value associated with the successful result.</param>
    /// <returns>A result indicating success, containing the specified value and a success type of "Ok".</returns>
    public static Result<T> Success(T value)
        => new() { IsSuccess = true, Value = value, SuccessType = SuccessType.Ok };

    /// <summary>
    /// Creates a successful result with the specified value and a success type of "Created".
    /// </summary>
    /// <param name="value">The value associated with the successful result.</param>
    /// <returns>A result indicating success, containing the specified value and a success type of "Created".</returns>
    public static Result<T> Created(T value)
        => new() { IsSuccess = true, Value = value, SuccessType = SuccessType.Created };

    /// <summary>
    /// Creates a successful result with no associated value and a success type of "NoContent".
    /// </summary>
    /// <returns>A result indicating success with a success type of "NoContent".</returns>
    public static Result<T> NoContent()
        => new() { IsSuccess = true, SuccessType = SuccessType.NoContent };

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the failed result, detailing what went wrong.</param>
    /// <returns>A result indicating failure, containing the specified error and no success type.</returns>
    public static Result<T> Failure(Error error)
        => new() { IsSuccess = false, SuccessType = SuccessType.None, Error = error };
}