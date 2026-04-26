namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public readonly record struct DressCodeColorId(Guid Value)
{
    public static DressCodeColorId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
