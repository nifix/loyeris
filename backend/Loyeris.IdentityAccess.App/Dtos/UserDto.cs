using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.App.Dtos;

/// <summary>
/// Read model returned when listing application users without sensitive authentication data.
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Phone,
    UserStatus Status,
    DateTimeOffset? EmailConfirmedAt,
    DateTimeOffset? LastLoginAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
