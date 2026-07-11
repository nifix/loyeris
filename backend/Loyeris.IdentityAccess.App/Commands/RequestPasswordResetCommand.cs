using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Requests a password reset link without disclosing whether an account exists.
/// </summary>
public record RequestPasswordResetCommand(
    string Email,
    string IpAddress,
    string UserAgent) : IRequest<Result<Unit>>;
