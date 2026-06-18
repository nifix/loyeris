using Loyeris.RentCollection.Core.Entities;
using Loyeris.RentCollection.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.RentCollection.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="RentReminder"/>.
/// </summary>
public class RentReminderConfiguration : IEntityTypeConfiguration<RentReminder>
{
    /// <summary>
    /// Applies table, reminder status, scheduling index, and deadline relationship settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<RentReminder> builder)
    {
        builder.ToTable("rent_reminders");
        builder.HasKey(reminder => reminder.Id);
        builder.HasIndex(reminder => new { reminder.Status, reminder.ScheduledFor });
        builder.HasIndex(reminder => reminder.RentDeadlineId);

        builder.Property(reminder => reminder.Channel).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(reminder => reminder.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(RentReminderStatus.Scheduled);
        builder.Property(reminder => reminder.RecipientEmail).HasMaxLength(320);
        builder.Property(reminder => reminder.Subject).HasMaxLength(240);
        builder.Property(reminder => reminder.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(reminder => reminder.UpdatedAt).HasDefaultValueSql("now()");

        builder
            .HasOne(reminder => reminder.RentDeadline)
            .WithMany(deadline => deadline.Reminders)
            .HasForeignKey(reminder => reminder.RentDeadlineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
