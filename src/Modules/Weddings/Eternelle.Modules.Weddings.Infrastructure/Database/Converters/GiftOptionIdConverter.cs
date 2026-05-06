using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class GiftOptionIdConverter()
    : ValueConverter<GiftOptionId, Guid>(id => id.Value, value => new GiftOptionId(value));
