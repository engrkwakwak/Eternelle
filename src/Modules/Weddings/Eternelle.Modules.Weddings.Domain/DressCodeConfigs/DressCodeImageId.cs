namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public readonly record struct DressCodeImageId(Guid Value)
{
    public static DressCodeImageId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
