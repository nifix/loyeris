using Loyeris.Shared.Results;

namespace Loyeris.Api.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Result{T}"/> instances,
/// enabling their conversion to HTTP responses.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> instance to an HTTP response represented by an <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The result of the operation to be converted into an HTTP response.</param>
    /// <typeparam name="T">The type of the value contained in the result when the operation succeeds.</typeparam>
    /// <returns>An <see cref="IResult"/> representing the corresponding HTTP response.</returns>
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        // If success
        if (result.IsSuccess)
        {
            return result.SuccessType switch
            {
                SuccessType.Created => Results.Created(string.Empty, result.Value),
                SuccessType.NoContent => Results.NoContent(),
                _ => Results.Ok(result.Value)
            };
        }

        // If failure without error
        if (result.Error is null)
            return Results.Problem("Unknown error");
        
        // If failure with error
        var statusCode = MapErrorStatusCode(result.Error.Type);
        return Results.Problem(
            title: result.Error.Code,
            detail: result.Error.Message,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = result.Error.Code
            }
        );
    }

    /// <summary>
    /// Maps an <see cref="ErrorType"/> to its corresponding HTTP status code.
    /// </summary>
    /// <param name="type">The error type that needs to be mapped to an HTTP status code.</param>
    /// <returns>The HTTP status code corresponding to the specified <see cref="ErrorType"/>.</returns>
    private static int MapErrorStatusCode(ErrorType type) => type switch
    {
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status400BadRequest
    };
}