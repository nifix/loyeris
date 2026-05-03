namespace Loyeris.Shared.Results;

/// <summary>
/// Represents an error that provides details about what went wrong in an operation.
/// </summary>
public record Error(string Code, string Message, ErrorType Type);