using Loyeris.IdentityAccess.Core.Enums;

namespace Loyeris.IdentityAccess.Core.Entities;

/// <summary>
/// Represents a person who can authenticate into Loyeris.
/// </summary>
public class AppUser
{
    /// <summary>
    /// Gets or sets the stable technical identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address displayed to the user and used for sign-in.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the normalized email used for uniqueness and lookup.
    /// </summary>
    public string NormalizedEmail { get; set; }

    /// <summary>
    /// Gets or sets the password hash. Plain text passwords are never stored.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the security version used to invalidate sessions after sensitive account changes.
    /// </summary>
    public string SecurityStamp { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the optional phone number attached to the profile.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the account lifecycle status.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Gets or sets when the user confirmed ownership of their email address.
    /// </summary>
    public DateTimeOffset? EmailConfirmedAt { get; set; }

    /// <summary>
    /// Gets or sets when the password was last changed.
    /// </summary>
    public DateTimeOffset? PasswordChangedAt { get; set; }

    /// <summary>
    /// Gets or sets the current number of consecutive failed sign-in attempts.
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Gets or sets when the temporary account lockout ends, if any.
    /// </summary>
    public DateTimeOffset? LockoutEndsAt { get; set; }

    /// <summary>
    /// Gets or sets the last successful sign-in timestamp, when known.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the workspaces directly owned by this user.
    /// </summary>
    public ICollection<Workspace> OwnedWorkspaces { get; set; } = new List<Workspace>();

    /// <summary>
    /// Gets or sets the workspace memberships granted to this user.
    /// </summary>
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = new List<WorkspaceMember>();

    /// <summary>
    /// Gets or sets the authentication sessions opened by this user.
    /// </summary>
    public ICollection<AuthSession> AuthSessions { get; set; } = new List<AuthSession>();

    /// <summary>
    /// Gets or sets the one-time authentication tokens issued to this user.
    /// </summary>
    public ICollection<AuthOneTimeToken> AuthOneTimeTokens { get; set; } = new List<AuthOneTimeToken>();

    /// <summary>
    /// Gets or sets the security events attached to this user.
    /// </summary>
    public ICollection<AuthEvent> AuthEvents { get; set; } = new List<AuthEvent>();

    /// <summary>
    /// Gets or sets the user's interface preferences.
    /// </summary>
    public UserPreference UserPreference { get; set; }
}
