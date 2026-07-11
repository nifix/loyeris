using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Replaces an account password after consuming a valid one-time reset token.
/// </summary>
public record ResetPasswordCommand(
    string Token,
    string NewPassword,
    string IpAddress,
    string UserAgent) : IRequest<Result<Unit>>;
