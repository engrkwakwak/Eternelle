using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Reminders;

public static class ReminderErrors
{
    public static Error NotFound(ReminderId id) =>
        Error.NotFound(
            "Reminders.NotFound",
            $"The reminder with the identifier {id.Value} was not found");

    public static readonly Error ReorderListMismatch =
        Error.Problem(
            "Reminders.ReorderListMismatch",
            "The provided reminder list does not match the existing reminders for this wedding.");
}
