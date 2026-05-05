using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.GetDressCodeConfig;

internal sealed class GetDressCodeConfigQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetDressCodeConfigQuery, DressCodeConfigResponse>
{
    public async Task<Result<DressCodeConfigResponse>> Handle(
        GetDressCodeConfigQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 dc.id           AS {nameof(FlatConfig.Id)},
                 dc.wedding_id   AS {nameof(FlatConfig.WeddingId)},
                 dc.description  AS {nameof(FlatConfig.Description)}
             FROM wedding.dress_code_configs dc
             WHERE dc.wedding_id = @WeddingId;

             SELECT
                 c.id            AS {nameof(DressCodeColorResponse.Id)},
                 c.color_hex     AS {nameof(DressCodeColorResponse.ColorHex)},
                 c.color_name    AS {nameof(DressCodeColorResponse.ColorName)},
                 c.display_order AS {nameof(DressCodeColorResponse.DisplayOrder)}
             FROM wedding.dress_code_colors c
             INNER JOIN wedding.dress_code_configs dc ON dc.id = c.dress_code_config_id
             WHERE dc.wedding_id = @WeddingId
             ORDER BY c.display_order ASC;

             SELECT
                 i.id            AS {nameof(DressCodeImageResponse.Id)},
                 i.image_url     AS {nameof(DressCodeImageResponse.ImageUrl)},
                 i.display_order AS {nameof(DressCodeImageResponse.DisplayOrder)}
             FROM wedding.dress_code_images i
             INNER JOIN wedding.dress_code_configs dc ON dc.id = i.dress_code_config_id
             WHERE dc.wedding_id = @WeddingId
             ORDER BY i.display_order ASC;
             """;

        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(
            new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        FlatConfig? flat = await multi.ReadFirstOrDefaultAsync<FlatConfig>();

        if (flat is null)
        {
            return Result.Failure<DressCodeConfigResponse>(
                DressCodeConfigErrors.NotFoundForWedding(query.WeddingId));
        }

        IEnumerable<DressCodeColorResponse> colors = await multi.ReadAsync<DressCodeColorResponse>();
        IEnumerable<DressCodeImageResponse> images = await multi.ReadAsync<DressCodeImageResponse>();

        var response = new DressCodeConfigResponse(
            flat.Id,
            flat.WeddingId,
            flat.Description,
            [.. colors],
            [.. images]);

        return Result.Success(response);
    }

    private sealed record FlatConfig(Guid Id, Guid WeddingId, string Description);
}
