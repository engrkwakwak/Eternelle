namespace Eternelle.Modules.Weddings.Domain.Weddings;

public readonly record struct SnapShareConfigId(Guid Value)
{
    public static SnapShareConfigId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
