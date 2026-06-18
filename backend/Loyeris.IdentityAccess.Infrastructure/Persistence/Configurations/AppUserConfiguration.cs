using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="AppUser"/>.
/// </summary>
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    /// <summary>
    /// Applies table, column, and index settings for application users.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("app_users");
        builder.HasKey(user => user.Id);
        builder.HasIndex(user => user.NormalizedEmail).IsUnique();

        builder.Property(user => user.Email).HasMaxLength(320).IsRequired();
        builder.Property(user => user.NormalizedEmail).HasMaxLength(320).IsRequired();
        builder.Property(user => user.PasswordHash).IsRequired();
        builder.Property(user => user.SecurityStamp).HasMaxLength(128).HasDefaultValueSql("gen_random_uuid()::text").IsRequired();
        builder.Property(user => user.FirstName).HasMaxLength(120);
        builder.Property(user => user.LastName).HasMaxLength(120);
        builder.Property(user => user.Phone).HasMaxLength(40);
        builder.Property(user => user.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(UserStatus.Invited);
        builder.Property(user => user.AccessFailedCount).HasDefaultValue(0);
        builder.Property(user => user.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(user => user.UpdatedAt).HasDefaultValueSql("now()");
    }
}
