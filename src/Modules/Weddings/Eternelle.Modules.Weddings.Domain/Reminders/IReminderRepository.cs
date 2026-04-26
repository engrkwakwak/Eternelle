using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.Reminders;

public interface IReminderRepository
{
    Task<Reminder?> GetAsync(ReminderId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Reminder>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(Reminder reminder);

    void Update(Reminder reminder);

    void Delete(Reminder reminder);
}
