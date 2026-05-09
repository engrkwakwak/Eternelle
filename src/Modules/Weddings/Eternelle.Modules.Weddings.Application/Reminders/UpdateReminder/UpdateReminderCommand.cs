using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Reminders.UpdateReminder;

public sealed record UpdateReminderCommand(
    Guid ReminderId,
    string Icon,
    string Title,
    string Body) : ICommand;
