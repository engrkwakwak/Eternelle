namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public readonly record struct GiftOptionId(Guid Value)
{
    public static GiftOptionId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
