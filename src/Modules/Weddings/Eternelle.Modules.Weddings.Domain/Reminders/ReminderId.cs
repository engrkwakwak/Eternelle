namespace Eternelle.Modules.Weddings.Domain.Reminders;

public readonly record struct ReminderId(Guid Value)
{
    public static ReminderId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
