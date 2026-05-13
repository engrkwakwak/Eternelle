using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeImage;

public sealed record RemoveDressCodeImageCommand(Guid WeddingId, Guid DressCodeConfigId, Guid DressCodeImageId) : ICommand;
