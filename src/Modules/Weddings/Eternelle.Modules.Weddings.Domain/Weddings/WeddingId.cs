namespace Eternelle.Modules.Weddings.Domain.Weddings;

public readonly record struct WeddingId(Guid Value)
{
    public static WeddingId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
