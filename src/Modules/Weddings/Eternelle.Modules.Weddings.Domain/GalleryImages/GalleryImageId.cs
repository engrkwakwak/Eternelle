namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public readonly record struct GalleryImageId(Guid Value)
{
    public static GalleryImageId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
