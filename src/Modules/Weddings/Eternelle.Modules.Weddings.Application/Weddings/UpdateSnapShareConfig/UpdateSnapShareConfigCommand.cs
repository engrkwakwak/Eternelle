using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateSnapShareConfig;

public sealed record UpdateSnapShareConfigCommand(
    Guid WeddingId,
    string? InstagramHandle,
    string? CtaText,
    bool Enabled) : ICommand;
