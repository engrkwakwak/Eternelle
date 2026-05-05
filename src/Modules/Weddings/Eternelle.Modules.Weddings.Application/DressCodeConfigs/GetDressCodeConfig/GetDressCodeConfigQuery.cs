using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.GetDressCodeConfig;

public sealed record GetDressCodeConfigQuery(Guid WeddingId) : IQuery<DressCodeConfigResponse>;
