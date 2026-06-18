using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="Workspace"/>.
/// </summary>
public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    /// <summary>
    /// Applies table, owner relationship, and workspace status settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("workspaces");
        builder.HasKey(workspace => workspace.Id);
        builder.HasIndex(workspace => workspace.OwnerUserId);

        builder.Property(workspace => workspace.Name).HasMaxLength(160).IsRequired();
        builder.Property(workspace => workspace.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(WorkspaceStatus.Active);
        builder.Property(workspace => workspace.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(workspace => workspace.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(workspace => workspace.OwnerUser)
            .WithMany(user => user.OwnedWorkspaces)
            .HasForeignKey(workspace => workspace.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
