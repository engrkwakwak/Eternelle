using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class VendorCreditIdConverter()
    : ValueConverter<VendorCreditId, Guid>(id => id.Value, value => new VendorCreditId(value));
