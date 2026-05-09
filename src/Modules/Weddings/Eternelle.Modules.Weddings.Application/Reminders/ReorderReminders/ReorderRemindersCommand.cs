using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Reminders.ReorderReminders;

public sealed record ReorderRemindersCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> ReminderIds) : ICommand;
