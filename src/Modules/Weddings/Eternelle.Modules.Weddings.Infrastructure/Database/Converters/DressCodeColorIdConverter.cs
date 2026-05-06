using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class DressCodeColorIdConverter()
    : ValueConverter<DressCodeColorId, Guid>(id => id.Value, value => new DressCodeColorId(value));
