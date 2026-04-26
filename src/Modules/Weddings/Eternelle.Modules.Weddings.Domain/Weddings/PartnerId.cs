namespace Eternelle.Modules.Weddings.Domain.Weddings;

public readonly record struct PartnerId(Guid Value)
{
    public static PartnerId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
