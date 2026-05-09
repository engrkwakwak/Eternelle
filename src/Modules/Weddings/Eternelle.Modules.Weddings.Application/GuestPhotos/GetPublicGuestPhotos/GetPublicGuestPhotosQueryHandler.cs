using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetPublicGuestPhotos;

internal sealed class GetPublicGuestPhotosQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetPublicGuestPhotosQuery, IReadOnlyList<GuestPhotoResponse>>
{
    public async Task<Result<IReadOnlyList<GuestPhotoResponse>>> Handle(
        GetPublicGuestPhotosQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        string sql =
            $"""
             SELECT
                 p.id             AS {nameof(GuestPhotoResponse.Id)},
                 p.src_url        AS {nameof(GuestPhotoResponse.SrcUrl)},
                 p.thumbnail_url  AS {nameof(GuestPhotoResponse.ThumbnailUrl)},
                 p.width_px       AS {nameof(GuestPhotoResponse.WidthPx)},
                 p.height_px      AS {nameof(GuestPhotoResponse.HeightPx)},
                 p.uploader_name  AS {nameof(GuestPhotoResponse.UploaderName)},
                 '{nameof(GuestPhotoStatus.Approved)}' AS {nameof(GuestPhotoResponse.Status)},
                 p.uploaded_at    AS {nameof(GuestPhotoResponse.UploadedAt)},
                 p.reviewed_at    AS {nameof(GuestPhotoResponse.ReviewedAt)}
             FROM wedding.guest_photos p
             WHERE p.wedding_id = @WeddingId
               AND p.status = {(int)GuestPhotoStatus.Approved}
             ORDER BY p.uploaded_at DESC
             """;

        IEnumerable<GuestPhotoResponse> photos =
            await connection.QueryAsync<GuestPhotoResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return photos.ToList();
    }
}
