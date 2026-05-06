using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eternelle.Modules.Weddings.Infrastructure.Database.Converters;

internal sealed class EntourageMemberIdConverter()
    : ValueConverter<EntourageMemberId, Guid>(id => id.Value, value => new EntourageMemberId(value));
