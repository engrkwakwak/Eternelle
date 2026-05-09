using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.GetGiftOptions;

internal sealed class GetGiftOptionsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetGiftOptionsQuery, IReadOnlyList<GiftOptionResponse>>
{
    public async Task<Result<IReadOnlyList<GiftOptionResponse>>> Handle(
        GetGiftOptionsQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 g.id             AS {nameof(GiftOptionResponse.Id)},
                 g.wedding_id     AS {nameof(GiftOptionResponse.WeddingId)},
                 g.title          AS {nameof(GiftOptionResponse.Title)},
                 g.description    AS {nameof(GiftOptionResponse.Description)},
                 g.display_mode   AS {nameof(GiftOptionResponse.DisplayMode)},
                 g.link_url       AS {nameof(GiftOptionResponse.LinkUrl)},
                 g.image_url      AS {nameof(GiftOptionResponse.ImageUrl)},
                 g.qr_image_url   AS {nameof(GiftOptionResponse.QrImageUrl)},
                 g.account_name   AS {nameof(GiftOptionResponse.AccountName)},
                 g.account_number AS {nameof(GiftOptionResponse.AccountNumber)},
                 g.account_type   AS {nameof(GiftOptionResponse.AccountType)},
                 g.display_order  AS {nameof(GiftOptionResponse.DisplayOrder)}
             FROM wedding.gift_options g
             WHERE g.wedding_id = @WeddingId
             ORDER BY g.display_order ASC, g.id ASC
             """;

        IEnumerable<GiftOptionResponse> giftOptions =
            await connection.QueryAsync<GiftOptionResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return giftOptions.ToList();
    }
}
