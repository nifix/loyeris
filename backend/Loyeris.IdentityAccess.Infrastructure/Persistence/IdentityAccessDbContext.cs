using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence;

/// <summary>
/// EF Core context responsible for identity, workspace, and user preference persistence.
/// </summary>
public class IdentityAccessDbContext(DbContextOptions<IdentityAccessDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the users that can authenticate into the application.
    /// </summary>
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    /// <summary>
    /// Gets the authenticated device or browser sessions.
    /// </summary>
    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    /// <summary>
    /// Gets the hashed refresh tokens issued for authentication sessions.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Gets the hashed one-time tokens used by security workflows.
    /// </summary>
    public DbSet<AuthOneTimeToken> AuthOneTimeTokens => Set<AuthOneTimeToken>();

    /// <summary>
    /// Gets the lightweight authentication security events.
    /// </summary>
    public DbSet<AuthEvent> AuthEvents => Set<AuthEvent>();

    /// <summary>
    /// Gets the interface preferences owned by users.
    /// </summary>
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    /// <summary>
    /// Gets the workspaces that partition customer data.
    /// </summary>
    public DbSet<Workspace> Workspaces => Set<Workspace>();

    /// <summary>
    /// Gets the memberships granting users access to workspaces.
    /// </summary>
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityAccessDbContext).Assembly);
        ApplySnakeCaseColumnNames(modelBuilder);
    }

    private static void ApplySnakeCaseColumnNames(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var chars = new List<char>(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];

            if (char.IsUpper(current) && i > 0)
            {
                var previous = value[i - 1];
                var hasNext = i + 1 < value.Length;

                if (previous != '_' && (!char.IsUpper(previous) || hasNext && !char.IsUpper(value[i + 1])))
                {
                    chars.Add('_');
                }
            }

            chars.Add(char.ToLowerInvariant(current));
        }

        return new string(chars.ToArray());
    }
}
