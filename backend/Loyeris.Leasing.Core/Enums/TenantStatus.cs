namespace Loyeris.Leasing.Core.Enums;

/// <summary>
/// Describes whether a tenant is active in the portfolio or archived.
/// </summary>
public enum TenantStatus
{
    /// <summary>
    /// The tenant can be linked to active rental workflows.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The tenant is retained for historical records only.
    /// </summary>
    Archived = 1
}
