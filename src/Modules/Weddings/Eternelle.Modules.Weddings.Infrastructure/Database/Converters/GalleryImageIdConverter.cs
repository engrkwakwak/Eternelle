using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class GalleryImageIdConverter()
    : ValueConverter<GalleryImageId, Guid>(id => id.Value, value => new GalleryImageId(value));
