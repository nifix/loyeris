using Loyeris.Leasing.Core.Enums;

namespace Loyeris.Leasing.App.Dtos;

/// <summary>
/// Read model returned when listing lease tenant links.
/// </summary>
public record LeaseTenantDto(
    Guid Id,
    Guid LeaseId,
    Guid TenantId,
    LeaseTenantRole Role,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
