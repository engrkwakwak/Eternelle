using Eternelle.Modules.Weddings.Domain.Weddings;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class PartnerIdConverter()
    : ValueConverter<PartnerId, Guid>(id => id.Value, value => new PartnerId(value));
