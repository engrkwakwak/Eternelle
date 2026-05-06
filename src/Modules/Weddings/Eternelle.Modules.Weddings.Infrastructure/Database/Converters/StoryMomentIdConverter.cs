using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class StoryMomentIdConverter()
    : ValueConverter<StoryMomentId, Guid>(id => id.Value, value => new StoryMomentId(value));
