namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public readonly record struct EntourageCoupleId(Guid Value)
{
    public static EntourageCoupleId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
