using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Represents an authenticated device or browser session for a user.
/// </summary>
public class AuthSession
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the session.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user that owns the session.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the lifecycle status of the session.
    /// </summary>
    public AuthSessionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the optional human-readable device label.
    /// </summary>
    public string DeviceLabel { get; set; }

    /// <summary>
    /// Gets or sets the user agent captured when the session was created.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the IP address captured when the session was created.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the session was last used.
    /// </summary>
    public DateTimeOffset? LastSeenAt { get; set; }

    /// <summary>
    /// Gets or sets when the session can no longer be refreshed.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the session was revoked, if applicable.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the business or security reason for revocation.
    /// </summary>
    public string RevokedReason { get; set; }

    /// <summary>
    /// Gets or sets the user navigation for this session.
    /// </summary>
    public AppUser User { get; set; }

    /// <summary>
    /// Gets or sets the refresh tokens issued for this session.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// Gets or sets the security events linked to this session.
    /// </summary>
    public ICollection<AuthEvent> AuthEvents { get; set; } = new List<AuthEvent>();
}
