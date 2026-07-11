using Loyeris.IdentityAccess.App.Dtos;
using Loyeris.Shared.Results;
using MediatR;

namespace Loyeris.IdentityAccess.App.Commands;

/// <summary>
/// Registers a new self-service Loyeris account.
/// </summary>
public record RegisterAccountCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    bool TermsAccepted,
    string IpAddress,
    string UserAgent) : IRequest<Result<RegisteredAccountDto>>;
