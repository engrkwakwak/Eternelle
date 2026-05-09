using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.GetVendorCredits;

internal sealed class GetVendorCreditsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetVendorCreditsQuery, IReadOnlyList<VendorCreditResponse>>
{
    public async Task<Result<IReadOnlyList<VendorCreditResponse>>> Handle(
        GetVendorCreditsQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 v.id               AS {nameof(VendorCreditResponse.Id)},
                 v.wedding_id       AS {nameof(VendorCreditResponse.WeddingId)},
                 v.name             AS {nameof(VendorCreditResponse.Name)},
                 v.role             AS {nameof(VendorCreditResponse.Role)},
                 v.website_url      AS {nameof(VendorCreditResponse.WebsiteUrl)},
                 v.image_url        AS {nameof(VendorCreditResponse.ImageUrl)},
                 v.instagram_handle AS {nameof(VendorCreditResponse.InstagramHandle)},
                 v.display_order    AS {nameof(VendorCreditResponse.DisplayOrder)}
             FROM wedding.vendor_credits v
             WHERE v.wedding_id = @WeddingId
             ORDER BY v.display_order ASC, v.id ASC
             """;

        IEnumerable<VendorCreditResponse> credits =
            await connection.QueryAsync<VendorCreditResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return Result.Success<IReadOnlyList<VendorCreditResponse>>([.. credits]);
    }
}
