using Loyeris.Messaging.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Messaging.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="NotificationPreference"/>.
/// </summary>
public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    /// <summary>
    /// Applies table, user uniqueness, and default notification setting values.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");
        builder.HasKey(preference => preference.Id);
        builder.HasIndex(preference => preference.UserId).IsUnique();

        builder.Property(preference => preference.RentOverdueEmailEnabled).HasDefaultValue(true);
        builder.Property(preference => preference.PaymentRecordedEmailEnabled).HasDefaultValue(true);
        builder.Property(preference => preference.LeaseEndingEmailEnabled).HasDefaultValue(false);
        builder.Property(preference => preference.LeaseEndingNoticeMonths).HasDefaultValue(3);
        builder.Property(preference => preference.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(preference => preference.UpdatedAt).HasDefaultValueSql("now()");
    }
}
