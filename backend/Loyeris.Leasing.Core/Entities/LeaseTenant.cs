using Loyeris.Leasing.Core.Enums;

namespace Loyeris.Leasing.Core.Entities;

/// <summary>
/// Links a tenant to a lease with a specific role.
/// </summary>
public class LeaseTenant
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the lease-tenant link.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the lease identifier.
    /// </summary>
    public Guid LeaseId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the tenant role within the lease.
    /// </summary>
    public LeaseTenantRole Role { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the lease navigation.
    /// </summary>
    public Lease Lease { get; set; }

    /// <summary>
    /// Gets or sets the tenant navigation.
    /// </summary>
    public Tenant Tenant { get; set; }
}
