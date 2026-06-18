using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="AuthOneTimeToken"/>.
/// </summary>
public class AuthOneTimeTokenConfiguration : IEntityTypeConfiguration<AuthOneTimeToken>
{
    /// <summary>
    /// Applies table, indexes, purpose conversion, and user relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<AuthOneTimeToken> builder)
    {
        builder.ToTable("auth_one_time_tokens");
        builder.HasKey(oneTimeToken => oneTimeToken.Id);
        builder.HasIndex(oneTimeToken => oneTimeToken.TokenHash).IsUnique();
        builder.HasIndex(oneTimeToken => oneTimeToken.ExpiresAt);
        builder
            .HasIndex(oneTimeToken => new { oneTimeToken.UserId, oneTimeToken.Purpose })
            .IsUnique()
            .HasFilter("consumed_at IS NULL AND revoked_at IS NULL");

        builder.Property(oneTimeToken => oneTimeToken.Purpose).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(oneTimeToken => oneTimeToken.TokenHash).HasMaxLength(128).IsRequired();
        builder.Property(oneTimeToken => oneTimeToken.SentToEmail).HasMaxLength(320);
        builder.Property(oneTimeToken => oneTimeToken.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(oneTimeToken => oneTimeToken.RequestedByIp).HasMaxLength(64);
        builder.Property(oneTimeToken => oneTimeToken.UserAgent).HasMaxLength(512);

        builder
            .HasOne(oneTimeToken => oneTimeToken.User)
            .WithMany(user => user.AuthOneTimeTokens)
            .HasForeignKey(oneTimeToken => oneTimeToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
