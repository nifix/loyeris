using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Confirms ownership of an account email address with a one-time token.
/// </summary>
public record VerifyEmailCommand(string Token, string IpAddress, string UserAgent) : IRequest<Result<Unit>>;
