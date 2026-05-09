using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.GetGalleryImages;

internal sealed class GetGalleryImagesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetGalleryImagesQuery, IReadOnlyList<GalleryImageResponse>>
{
    public async Task<Result<IReadOnlyList<GalleryImageResponse>>> Handle(
        GetGalleryImagesQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 g.id             AS {nameof(GalleryImageResponse.Id)},
                 g.wedding_id     AS {nameof(GalleryImageResponse.WeddingId)},
                 g.src_url        AS {nameof(GalleryImageResponse.SrcUrl)},
                 g.alt_text       AS {nameof(GalleryImageResponse.AltText)},
                 g.width_px       AS {nameof(GalleryImageResponse.WidthPx)},
                 g.height_px      AS {nameof(GalleryImageResponse.HeightPx)},
                 g.caption        AS {nameof(GalleryImageResponse.Caption)},
                 g.display_order  AS {nameof(GalleryImageResponse.DisplayOrder)},
                 g.created_at     AS {nameof(GalleryImageResponse.CreatedAtUtc)}
             FROM wedding.gallery_images g
             WHERE g.wedding_id = @WeddingId
             ORDER BY g.display_order ASC, g.id ASC
             """;

        IEnumerable<GalleryImageResponse> images =
            await connection.QueryAsync<GalleryImageResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return Result.Success<IReadOnlyList<GalleryImageResponse>>([.. images]);
    }
}
