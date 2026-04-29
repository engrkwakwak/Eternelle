using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

internal sealed class GetWeddingQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetWeddingQuery, WeddingResponse>
{
    public async Task<Result<WeddingResponse>> Handle(GetWeddingQuery query, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 p.id                AS {nameof(WeddingResponse.Id)},
                 p.tenant_id         AS {nameof(WeddingResponse.TenantId)},
                 p.wedding_date      AS {nameof(WeddingResponse.WeddingDate)},
                 p.hashtag           AS {nameof(WeddingResponse.Hashtag)},
                 p.created_at        AS {nameof(WeddingResponse.CreatedAtUtc)},
                 p.updated_at        AS {nameof(WeddingResponse.UpdatedAtUtc)},
                 pa.id               AS {nameof(PartnerResponse.PartnerId)},
                 pa.partner_number   AS {nameof(PartnerResponse.PartnerNumber)},
                 pa.first_name       AS {nameof(PartnerResponse.FirstName)},
                 pa.last_name        AS {nameof(PartnerResponse.LastName)},
                 pa.bio              AS {nameof(PartnerResponse.Bio)},
                 pa.image_url        AS {nameof(PartnerResponse.ImageUrl)},
                 ss.id               AS {nameof(SnapShareResponse.SnapShareId)},
                 ss.instagram_handle AS {nameof(SnapShareResponse.InstagramHandle)},
                 ss.cta_text         AS {nameof(SnapShareResponse.CtaText)},
                 ss.enabled          AS {nameof(SnapShareResponse.Enabled)}
             FROM wedding.profiles p
             LEFT JOIN wedding.partners pa ON pa.wedding_id = p.id
             LEFT JOIN wedding.snap_share_configs ss ON ss.wedding_id = p.id
             WHERE p.id = @WeddingId
             """;

        Dictionary<Guid, WeddingResponse> weddingDictionary = [];

        await connection.QueryAsync<WeddingResponse, PartnerResponse?, SnapShareResponse?, WeddingResponse>(
            sql,
            (wedding, partner, snapShare) =>
            {
                if (weddingDictionary.TryGetValue(wedding.Id, out WeddingResponse? existingWedding))
                {
                    wedding = existingWedding;
                }
                else
                {
                    weddingDictionary.Add(wedding.Id, wedding);
                }

                if (partner is not null)
                {
                    wedding.Partners.Add(partner);
                }

                if (snapShare is not null && wedding.SnapShare is null)
                {
                    wedding.SnapShare = snapShare;
                }

                return wedding;
            },
            query,
            splitOn: $"{nameof(PartnerResponse.PartnerId)},{nameof(SnapShareResponse.SnapShareId)}");

        if (!weddingDictionary.TryGetValue(query.WeddingId, out WeddingResponse? weddingResponse))
        {
            return Result.Failure<WeddingResponse>(WeddingErrors.NotFound(new WeddingId(query.WeddingId)));
        }

        return weddingResponse;
    }
}
