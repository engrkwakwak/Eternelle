namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public readonly record struct EntourageGroupId(Guid Value)
{
    public static EntourageGroupId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
