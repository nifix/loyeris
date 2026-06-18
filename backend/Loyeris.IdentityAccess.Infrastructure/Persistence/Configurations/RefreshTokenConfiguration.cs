using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="RefreshToken"/>.
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <summary>
    /// Applies table, indexes, active-token constraint, and session relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(refreshToken => refreshToken.Id);
        builder.HasIndex(refreshToken => refreshToken.TokenHash).IsUnique();
        builder.HasIndex(refreshToken => refreshToken.ExpiresAt);
        builder
            .HasIndex(refreshToken => refreshToken.SessionId)
            .IsUnique()
            .HasFilter("consumed_at IS NULL AND revoked_at IS NULL");

        builder.Property(refreshToken => refreshToken.TokenHash).HasMaxLength(128).IsRequired();
        builder.Property(refreshToken => refreshToken.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(refreshToken => refreshToken.RevokedReason).HasMaxLength(240);
        builder.Property(refreshToken => refreshToken.CreatedByIp).HasMaxLength(64);
        builder.Property(refreshToken => refreshToken.ConsumedByIp).HasMaxLength(64);

        builder
            .HasOne(refreshToken => refreshToken.Session)
            .WithMany(session => session.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(refreshToken => refreshToken.ReplacedByToken)
            .WithMany()
            .HasForeignKey(refreshToken => refreshToken.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
