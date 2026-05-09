namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.GetDressCodeConfig;

public sealed record DressCodeConfigResponse(
    Guid Id,
    Guid WeddingId,
    string Description,
    IReadOnlyList<DressCodeColorResponse> Colors,
    IReadOnlyList<DressCodeImageResponse> Images);
