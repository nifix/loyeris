using Loyeris.IdentityAccess.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.IdentityAccess.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="UserPreference"/>.
/// </summary>
public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    /// <summary>
    /// Applies table, defaults, and one-to-one user relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("user_preferences");
        builder.HasKey(preference => preference.Id);
        builder.HasIndex(preference => preference.UserId).IsUnique();

        builder.Property(preference => preference.Language).HasMaxLength(10).HasDefaultValue("fr").IsRequired();
        builder.Property(preference => preference.Currency).HasMaxLength(3).HasDefaultValue("EUR").IsRequired();
        builder.Property(preference => preference.Theme).HasMaxLength(30).HasDefaultValue("corporate").IsRequired();
        builder.Property(preference => preference.CompactTables).HasDefaultValue(false);
        builder.Property(preference => preference.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(preference => preference.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(preference => preference.User)
            .WithOne(user => user.UserPreference)
            .HasForeignKey<UserPreference>(preference => preference.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
