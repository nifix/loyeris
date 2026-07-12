namespace Loyeris.Leasing.App.Dtos;

/// <summary>
/// Tenant option that can be assigned to a rental lot.
/// </summary>
public record AvailableTenantDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsCurrent);
