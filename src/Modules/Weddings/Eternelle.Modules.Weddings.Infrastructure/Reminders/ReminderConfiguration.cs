using Eternelle.Modules.Weddings.Domain.Reminders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.Reminders;

internal sealed class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("reminders");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.WeddingId).IsRequired();
        builder.HasIndex(r => r.WeddingId)
            .HasDatabaseName("ix_reminders_wedding_id");

        builder.Property(r => r.Icon)
            .IsRequired()
            .HasMaxLength(Reminder.MaxIconLength);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(Reminder.MaxTitleLength);

        builder.Property(r => r.Body)
            .IsRequired()
            .HasMaxLength(Reminder.MaxBodyLength);

        builder.Property(r => r.DisplayOrder).IsRequired();
    }
}
