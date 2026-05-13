using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeColor;

public sealed record RemoveDressCodeColorCommand(Guid WeddingId, Guid DressCodeConfigId, Guid DressCodeColorId) : ICommand;
