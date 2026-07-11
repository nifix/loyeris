using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Revokes the browser session identified by a refresh token.
/// </summary>
public record LogoutCommand(string RefreshToken, string IpAddress, string UserAgent) : IRequest<Result<Unit>>;
