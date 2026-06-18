using Loyeris.Messaging.Core.Entities;
using Loyeris.Messaging.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyeris.Messaging.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for <see cref="OutboxMessage"/>.
/// </summary>
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    /// <summary>
    /// Applies table, JSON payload, status, and processing queue index settings.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(message => message.Id);
        builder.HasIndex(message => new { message.Status, message.AvailableAt });

        builder.Property(message => message.Type).HasMaxLength(160).IsRequired();
        builder.Property(message => message.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(message => message.Status).HasConversion<string>().HasMaxLength(30).HasDefaultValue(OutboxMessageStatus.Pending);
        builder.Property(message => message.CreatedAt).HasDefaultValueSql("now()");
        builder.Property(message => message.UpdatedAt).HasDefaultValueSql("now()");
    }
}
