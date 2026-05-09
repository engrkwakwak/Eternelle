using Eternelle.Modules.Weddings.Domain.Reminders;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.Reminders;

internal sealed class ReminderRepository(WeddingsDbContext context) : IReminderRepository
{
    public async Task<Reminder?> GetAsync(ReminderId id, CancellationToken cancellationToken = default)
    {
        return await context.Reminders
            .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Reminder>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.Reminders
            .Where(r => r.WeddingId == weddingId)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public void Insert(Reminder reminder)
    {
        context.Reminders.Add(reminder);
    }

    public void Update(Reminder reminder)
    {
        context.Reminders.Update(reminder);
    }

    public void Delete(Reminder reminder)
    {
        context.Reminders.Remove(reminder);
    }
}
