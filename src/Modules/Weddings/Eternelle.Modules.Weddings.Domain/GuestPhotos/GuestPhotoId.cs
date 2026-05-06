namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public readonly record struct GuestPhotoId(Guid Value)
{
    public static GuestPhotoId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
