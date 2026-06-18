using Loyeris.Leasing.Core.Enums;

namespace Loyeris.Leasing.App.Dtos;

/// <summary>
/// Read model returned when listing tenants.
/// </summary>
public record TenantDto(
    Guid Id,
    Guid WorkspaceId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    TenantStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ArchivedAt);
