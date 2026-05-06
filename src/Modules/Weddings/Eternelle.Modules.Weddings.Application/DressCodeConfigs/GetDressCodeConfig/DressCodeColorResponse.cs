namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.GetDressCodeConfig;

public sealed record DressCodeColorResponse(
    Guid Id,
    string ColorHex,
    string ColorName,
    int DisplayOrder);
