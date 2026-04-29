using System.Data;
using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.GetEntourageGroups;

internal sealed class GetEntourageGroupsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetEntourageGroupsQuery, IReadOnlyList<EntourageGroupResponse>>
{
    public async Task<Result<IReadOnlyList<EntourageGroupResponse>>> Handle(
        GetEntourageGroupsQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string groupsSql =
            $"""
             SELECT
                 g.id            AS {nameof(EntourageGroupResponse.GroupId)},
                 g.wedding_id    AS {nameof(EntourageGroupResponse.WeddingId)},
                 g.label         AS {nameof(EntourageGroupResponse.Label)},
                 g.subtitle      AS {nameof(EntourageGroupResponse.Subtitle)},
                 g.group_type    AS {nameof(EntourageGroupResponse.GroupType)},
                 g.render_as     AS {nameof(EntourageGroupResponse.RenderAs)},
                 g.display_order AS {nameof(EntourageGroupResponse.DisplayOrder)}
             FROM wedding.entourage_groups g
             WHERE g.wedding_id = @WeddingId
             ORDER BY g.display_order ASC
             """;

        const string membersSql =
            $"""
             SELECT
                 m.id            AS {nameof(EntourageMemberResponse.MemberId)},
                 m.group_id      AS {nameof(EntourageMemberResponse.GroupId)},
                 m.name          AS {nameof(EntourageMemberResponse.Name)},
                 m.role          AS {nameof(EntourageMemberResponse.Role)},
                 m.image_url     AS {nameof(EntourageMemberResponse.ImageUrl)},
                 m.message       AS {nameof(EntourageMemberResponse.Message)},
                 m.note          AS {nameof(EntourageMemberResponse.Note)},
                 m.seed          AS {nameof(EntourageMemberResponse.Seed)},
                 m.display_order AS {nameof(EntourageMemberResponse.DisplayOrder)}
             FROM wedding.entourage_members m
             WHERE m.wedding_id = @WeddingId
             ORDER BY m.display_order ASC
             """;

        const string couplesSql =
            $"""
             SELECT
                 c.id            AS {nameof(EntourageCoupleResponse.CoupleId)},
                 c.group_id      AS {nameof(EntourageCoupleResponse.GroupId)},
                 c.member_a_id   AS {nameof(EntourageCoupleResponse.MemberAId)},
                 c.member_b_id   AS {nameof(EntourageCoupleResponse.MemberBId)},
                 c.note          AS {nameof(EntourageCoupleResponse.Note)},
                 c.display_order AS {nameof(EntourageCoupleResponse.DisplayOrder)}
             FROM wedding.entourage_couples c
             INNER JOIN wedding.entourage_groups g ON g.id = c.group_id
             WHERE g.wedding_id = @WeddingId
             ORDER BY c.display_order ASC
             """;

        var param = new { query.WeddingId };

        // RepeatableRead ensures all three reads see the same committed snapshot —
        // a member or couple added between queries will not appear in one set but not
        // the other, preventing orphaned rows in the assembled tree.
        await using DbTransaction transaction =
            await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

        List<EntourageGroupResponse> groups =
        [
            .. await connection.QueryAsync<EntourageGroupResponse>(
                new CommandDefinition(groupsSql, param, transaction, cancellationToken: cancellationToken))
        ];

        if (groups.Count == 0)
        {
            await transaction.CommitAsync(cancellationToken);
            return new List<EntourageGroupResponse>();
        }

        var groupsById = groups.ToDictionary(g => g.GroupId);

        IEnumerable<EntourageMemberResponse> members = await connection.QueryAsync<EntourageMemberResponse>(
            new CommandDefinition(membersSql, param, transaction, cancellationToken: cancellationToken));

        foreach (EntourageMemberResponse member in members)
        {
            if (groupsById.TryGetValue(member.GroupId, out EntourageGroupResponse? group))
            {
                group.Members.Add(member);
            }
        }

        IEnumerable<EntourageCoupleResponse> couples = await connection.QueryAsync<EntourageCoupleResponse>(
            new CommandDefinition(couplesSql, param, transaction, cancellationToken: cancellationToken));

        foreach (EntourageCoupleResponse couple in couples)
        {
            if (groupsById.TryGetValue(couple.GroupId, out EntourageGroupResponse? group))
            {
                group.Couples.Add(couple);
            }
        }

        await transaction.CommitAsync(cancellationToken);

        return groups;
    }
}
