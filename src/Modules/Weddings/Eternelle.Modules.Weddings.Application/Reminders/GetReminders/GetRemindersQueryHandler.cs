using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.Reminders.GetReminders;

internal sealed class GetRemindersQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetRemindersQuery, IReadOnlyList<ReminderResponse>>
{
    public async Task<Result<IReadOnlyList<ReminderResponse>>> Handle(
        GetRemindersQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 r.id            AS {nameof(ReminderResponse.Id)},
                 r.wedding_id    AS {nameof(ReminderResponse.WeddingId)},
                 r.icon          AS {nameof(ReminderResponse.Icon)},
                 r.title         AS {nameof(ReminderResponse.Title)},
                 r.body          AS {nameof(ReminderResponse.Body)},
                 r.display_order AS {nameof(ReminderResponse.DisplayOrder)}
             FROM wedding.reminders r
             WHERE r.wedding_id = @WeddingId
             ORDER BY r.display_order ASC, r.id ASC
             """;

        IEnumerable<ReminderResponse> reminders =
            await connection.QueryAsync<ReminderResponse>(
                new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        return Result.Success<IReadOnlyList<ReminderResponse>>([.. reminders]);
    }
}
