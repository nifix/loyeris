namespace Loyeris.Leasing.Core.Enums;

/// <summary>
/// Describes how a tenant is related to a lease.
/// </summary>
public enum LeaseTenantRole
{
    /// <summary>
    /// Main tenant displayed by default in operational screens.
    /// </summary>
    Primary = 0,

    /// <summary>
    /// Additional occupant sharing responsibility for the lease.
    /// </summary>
    CoTenant = 1,

    /// <summary>
    /// Guarantor linked to the lease but not occupying the lot.
    /// </summary>
    Guarantor = 2
}
