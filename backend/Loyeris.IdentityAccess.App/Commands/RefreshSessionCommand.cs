using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Rotates a refresh token and issues a new JWT access token.
/// </summary>
public record RefreshSessionCommand(
    string RefreshToken,
    string IpAddress,
    string UserAgent) : IRequest<Result<AuthenticationSessionDto>>;
