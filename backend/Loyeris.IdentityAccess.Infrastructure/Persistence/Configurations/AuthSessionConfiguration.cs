using Loyeris.IdentityAccess.Core.Entities;
using Loyeris.IdentityAccess.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="AuthSession"/>.
/// </summary>
public class AuthSessionConfiguration : IEntityTypeConfiguration<AuthSession>
{
    /// <summary>
    /// Applies table, indexes, status conversion, and user relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<AuthSession> builder)
    {
        builder.ToTable("auth_sessions");
        builder.HasKey(session => session.Id);
        builder.HasIndex(session => new { session.UserId, session.Status, session.LastSeenAt });
        builder.HasIndex(session => new { session.Status, session.ExpiresAt });

        builder.Property(session => session.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(AuthSessionStatus.Active);
        builder.Property(session => session.DeviceLabel).HasMaxLength(160);
        builder.Property(session => session.UserAgent).HasMaxLength(512);
        builder.Property(session => session.IpAddress).HasMaxLength(64);
        builder.Property(session => session.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(session => session.RevokedReason).HasMaxLength(240);

        builder
            .HasOne(session => session.User)
            .WithMany(user => user.AuthSessions)
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
