using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Queries;

/// <summary>
/// Checks whether a password reset token can still be consumed without changing its state.
/// </summary>
public record ValidatePasswordResetTokenQuery(string Token) : IRequest<Result<Unit>>;
