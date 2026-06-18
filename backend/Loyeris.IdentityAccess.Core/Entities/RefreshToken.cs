namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Stores a hashed refresh token used to renew stateless JWT access tokens.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the refresh token.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the authentication session this token belongs to.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Gets or sets the cryptographic hash of the refresh token.
    /// </summary>
    public string TokenHash { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the refresh token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was consumed during rotation.
    /// </summary>
    public DateTimeOffset? ConsumedAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was explicitly revoked.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Gets or sets the business or security reason for revocation.
    /// </summary>
    public string RevokedReason { get; set; }

    /// <summary>
    /// Gets or sets the token that replaced this token during rotation.
    /// </summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>
    /// Gets or sets the IP address that created the token.
    /// </summary>
    public string CreatedByIp { get; set; }

    /// <summary>
    /// Gets or sets the IP address that consumed the token.
    /// </summary>
    public string ConsumedByIp { get; set; }

    /// <summary>
    /// Gets or sets the session navigation for this token.
    /// </summary>
    public AuthSession Session { get; set; }

    /// <summary>
    /// Gets or sets the replacement token navigation.
    /// </summary>
    public RefreshToken ReplacedByToken { get; set; }
}
