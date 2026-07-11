using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Authenticates an account and creates a refreshable browser session.
/// </summary>
public record LoginCommand(
    string Email,
    string Password,
    bool RememberMe,
    string IpAddress,
    string UserAgent) : IRequest<Result<AuthenticationSessionDto>>;
