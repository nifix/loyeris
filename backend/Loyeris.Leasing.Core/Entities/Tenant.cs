using Loyeris.Leasing.Core.Enums;

namespace Loyeris.Leasing.Core.Entities;

/// <summary>
/// Represents a person who rents, rented, or guarantees a managed lot.
/// </summary>
public class Tenant
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the tenant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workspace that owns this tenant record.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Gets or sets the tenant's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the tenant's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the tenant's optional email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the tenant's optional phone number.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets whether the tenant is active or archived.
    /// </summary>
    public TenantStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the tenant was archived, if applicable.
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the lease relationships for this tenant.
    /// </summary>
    public ICollection<LeaseTenant> LeaseTenants { get; set; } = new List<LeaseTenant>();
}
