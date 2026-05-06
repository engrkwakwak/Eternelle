using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class DressCodeImageIdConverter()
    : ValueConverter<DressCodeImageId, Guid>(id => id.Value, value => new DressCodeImageId(value));
