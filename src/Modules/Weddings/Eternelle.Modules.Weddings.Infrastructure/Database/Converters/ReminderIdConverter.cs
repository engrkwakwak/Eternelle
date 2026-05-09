using Eternelle.Modules.Weddings.Domain.Reminders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class ReminderIdConverter()
    : ValueConverter<ReminderId, Guid>(id => id.Value, value => new ReminderId(value));
