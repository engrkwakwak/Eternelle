namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

public readonly record struct VendorCreditId(Guid Value)
{
    public static VendorCreditId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
