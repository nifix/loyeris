using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="WorkspaceMember"/>.
/// </summary>
public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    /// <summary>
    /// Applies table, role, and unique membership settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("workspace_members");
        builder.HasKey(member => member.Id);
        builder.HasIndex(member => new { member.WorkspaceId, member.UserId }).IsUnique();
        builder.HasIndex(member => member.UserId);

        builder.Property(member => member.Role).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(member => member.JoinedAt).HasDefaultValueSql("now()");
        builder.Property(member => member.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(member => member.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(member => member.Workspace)
            .WithMany(workspace => workspace.Members)
            .HasForeignKey(member => member.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(member => member.User)
            .WithMany(user => user.WorkspaceMembers)
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
