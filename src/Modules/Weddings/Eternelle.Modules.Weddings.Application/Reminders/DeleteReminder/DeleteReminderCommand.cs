using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Reminders.DeleteReminder;

public sealed record DeleteReminderCommand(Guid ReminderId) : ICommand;
