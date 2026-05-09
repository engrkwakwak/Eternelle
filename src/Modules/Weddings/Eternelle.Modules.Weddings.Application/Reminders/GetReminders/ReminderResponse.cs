namespace Eternelle.Modules.Weddings.Application.Reminders.GetReminders;

public sealed record ReminderResponse(
    Guid Id,
    Guid WeddingId,
    string Icon,
    string Title,
    string Body,
    int DisplayOrder);
