using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Reminders.GetReminders;

public sealed record GetRemindersQuery(Guid WeddingId) : IQuery<IReadOnlyList<ReminderResponse>>;
