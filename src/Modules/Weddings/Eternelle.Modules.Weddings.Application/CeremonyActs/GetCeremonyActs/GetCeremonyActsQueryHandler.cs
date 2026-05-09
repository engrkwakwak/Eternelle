using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.GetCeremonyActs;

internal sealed class GetCeremonyActsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCeremonyActsQuery, IReadOnlyList<CeremonyActResponse>>
{
    public async Task<Result<IReadOnlyList<CeremonyActResponse>>> Handle(
        GetCeremonyActsQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 a.id            AS {nameof(CeremonyActResponse.Id)},
                 a.wedding_id    AS {nameof(CeremonyActResponse.WeddingId)},
                 a.name          AS {nameof(CeremonyActResponse.Name)},
                 a.description   AS {nameof(CeremonyActResponse.Description)},
                 a.icon          AS {nameof(CeremonyActResponse.Icon)},
                 a.act_time      AS {nameof(CeremonyActResponse.ActTime)},
                 a.display_order AS {nameof(CeremonyActResponse.DisplayOrder)}
             FROM wedding.ceremony_acts a
             WHERE a.wedding_id = @WeddingId
             ORDER BY a.display_order ASC, a.id ASC
             """;

        IEnumerable<CeremonyActResponse> acts =
            await connection.QueryAsync<CeremonyActResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return Result.Success<IReadOnlyList<CeremonyActResponse>>([.. acts]);
    }
}
