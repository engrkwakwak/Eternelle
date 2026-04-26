namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

public readonly record struct CeremonyActId(Guid Value)
{
    public static CeremonyActId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
