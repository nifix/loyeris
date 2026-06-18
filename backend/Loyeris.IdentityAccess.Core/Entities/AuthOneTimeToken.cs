using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Stores a hashed token for a single security workflow such as email confirmation or password reset.
/// </summary>
public class AuthOneTimeToken
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the one-time token.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user that received the token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the security workflow this token belongs to.
    /// </summary>
    public OneTimeTokenPurpose Purpose { get; set; }

    /// <summary>
    /// Gets or sets the cryptographic hash of the one-time token.
    /// </summary>
    public string TokenHash { get; set; }

    /// <summary>
    /// Gets or sets the email address the token was sent to.
    /// </summary>
    public string SentToEmail { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was consumed.
    /// </summary>
    public DateTimeOffset? ConsumedAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was explicitly revoked.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address that requested the token.
    /// </summary>
    public string RequestedByIp { get; set; }

    /// <summary>
    /// Gets or sets the user agent captured when the token was requested.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the user navigation for this one-time token.
    /// </summary>
    public AppUser User { get; set; }
}
