using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class GuestPhotoIdConverter()
    : ValueConverter<GuestPhotoId, Guid>(id => id.Value, value => new GuestPhotoId(value));
