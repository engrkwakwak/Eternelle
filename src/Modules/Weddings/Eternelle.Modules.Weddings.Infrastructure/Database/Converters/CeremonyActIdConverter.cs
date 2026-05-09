using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class CeremonyActIdConverter()
    : ValueConverter<CeremonyActId, Guid>(id => id.Value, value => new CeremonyActId(value));
