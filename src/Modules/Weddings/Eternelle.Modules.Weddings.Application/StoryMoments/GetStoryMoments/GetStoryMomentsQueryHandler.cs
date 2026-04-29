using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.GetStoryMoments;

internal sealed class GetStoryMomentsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetStoryMomentsQuery, IReadOnlyList<StoryMomentResponse>>
{
    public async Task<Result<IReadOnlyList<StoryMomentResponse>>> Handle(
        GetStoryMomentsQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 s.id            AS {nameof(StoryMomentResponse.Id)},
                 s.wedding_id    AS {nameof(StoryMomentResponse.WeddingId)},
                 s.title         AS {nameof(StoryMomentResponse.Title)},
                 s.story_date    AS {nameof(StoryMomentResponse.StoryDate)},
                 s.description   AS {nameof(StoryMomentResponse.Description)},
                 s.image_url     AS {nameof(StoryMomentResponse.ImageUrl)},
                 s.display_order AS {nameof(StoryMomentResponse.DisplayOrder)}
             FROM wedding.story_moments s
             WHERE s.wedding_id = @WeddingId
             ORDER BY s.display_order ASC
             """;

        IEnumerable<StoryMomentResponse> storyMoments =
            await connection.QueryAsync<StoryMomentResponse>(sql, query);

        return storyMoments.ToList();
    }
}
