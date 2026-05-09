using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeColor;

public sealed record RemoveDressCodeColorCommand(Guid DressCodeColorId) : ICommand;
