namespace Loyeris.Portfolio.App.Dtos;

/// <summary>
/// Read model returned when listing SCI associates.
/// </summary>
public record SciAssociateDto(
    Guid Id,
    Guid SciId,
    string FirstName,
    string LastName,
    string Email,
    int? SharesCount,
    decimal? OwnershipPercentage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
