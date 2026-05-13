using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateSnapShareConfig;

public sealed record UpdateSnapShareConfigCommand(
    Guid WeddingId,
    string? InstagramHandle,
    string? CtaText,
    bool Enabled,
    SnapShareModerationMode ModerationMode,
    bool UploaderNameRequired) : ICommand;
