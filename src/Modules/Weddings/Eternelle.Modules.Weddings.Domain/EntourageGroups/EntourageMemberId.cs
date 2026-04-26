namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public readonly record struct EntourageMemberId(Guid Value)
{
    public static EntourageMemberId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
