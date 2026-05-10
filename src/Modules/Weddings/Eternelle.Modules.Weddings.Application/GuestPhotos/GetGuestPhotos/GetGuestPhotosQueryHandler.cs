using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetGuestPhotos;

internal sealed class GetGuestPhotosQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetGuestPhotosQuery, IReadOnlyList<GuestPhotoResponse>>
{
    public async Task<Result<IReadOnlyList<GuestPhotoResponse>>> Handle(
        GetGuestPhotosQuery query,
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
                 CASE p.status
                     WHEN {(int)GuestPhotoStatus.Pending}  THEN '{nameof(GuestPhotoStatus.Pending)}'
                     WHEN {(int)GuestPhotoStatus.Approved} THEN '{nameof(GuestPhotoStatus.Approved)}'
                     WHEN {(int)GuestPhotoStatus.Rejected} THEN '{nameof(GuestPhotoStatus.Rejected)}'
                     WHEN {(int)GuestPhotoStatus.OverLimit} THEN '{nameof(GuestPhotoStatus.OverLimit)}'
                     ELSE 'Unknown'
                 END              AS {nameof(GuestPhotoResponse.Status)},
                 p.uploaded_at    AS {nameof(GuestPhotoResponse.UploadedAt)},
                 p.reviewed_at    AS {nameof(GuestPhotoResponse.ReviewedAt)}
             FROM wedding.guest_photos p
             WHERE p.wedding_id = @WeddingId
               AND (@Status IS NULL OR p.status = @Status)
             ORDER BY p.uploaded_at DESC
             """;

        IEnumerable<GuestPhotoResponse> photos =
            await connection.QueryAsync<GuestPhotoResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return photos.ToList();
    }
}
