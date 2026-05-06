using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Reminders.CreateReminder;

public sealed record CreateReminderCommand(
    Guid WeddingId,
    string Icon,
    string Title,
    string Body) : ICommand<Guid>;
