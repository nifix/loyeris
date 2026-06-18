using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="AuthEvent"/>.
/// </summary>
public class AuthEventConfiguration : IEntityTypeConfiguration<AuthEvent>
{
    /// <summary>
    /// Applies table, indexes, type conversion, and optional relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<AuthEvent> builder)
    {
        builder.ToTable("auth_events");
        builder.HasKey(authEvent => authEvent.Id);
        builder.HasIndex(authEvent => new { authEvent.UserId, authEvent.OccurredAt });
        builder.HasIndex(authEvent => new { authEvent.NormalizedEmail, authEvent.OccurredAt });
        builder.HasIndex(authEvent => new { authEvent.Type, authEvent.OccurredAt });

        builder.Property(authEvent => authEvent.NormalizedEmail).HasMaxLength(320);
        builder.Property(authEvent => authEvent.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(authEvent => authEvent.OccurredAt).HasDefaultValueSql("now()");
        builder.Property(authEvent => authEvent.IpAddress).HasMaxLength(64);
        builder.Property(authEvent => authEvent.UserAgent).HasMaxLength(512);
        builder.Property(authEvent => authEvent.FailureReason).HasMaxLength(160);
        builder.Property(authEvent => authEvent.MetadataJson).HasColumnType("jsonb");

        builder
            .HasOne(authEvent => authEvent.User)
            .WithMany(user => user.AuthEvents)
            .HasForeignKey(authEvent => authEvent.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(authEvent => authEvent.Session)
            .WithMany(session => session.AuthEvents)
            .HasForeignKey(authEvent => authEvent.SessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
