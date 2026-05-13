using System.Data.Common;
using Dapper;
using Eternelle.Common.Application.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetUploadContext;

internal sealed class GetUploadContextQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUploadContextQuery, UploadContextResponse>
{
    public async Task<Result<UploadContextResponse>> Handle(
        GetUploadContextQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        // Resolve the wedding from the upload token stored on SnapShareConfig.
        // Returns a generic not-found so callers cannot probe token validity
        // (same error whether the token never existed or the wedding is deleted).
        const string sql =
            """
            SELECT
                sc.wedding_id               AS WeddingId,
                sc.uploader_name_required   AS UploaderNameRequired
            FROM wedding.snap_share_configs sc
            WHERE sc.upload_token = @UploadToken
            """;

        UploadContextResponse? context = await connection.QuerySingleOrDefaultAsync<UploadContextResponse>(
            new CommandDefinition(sql, query, cancellationToken: cancellationToken));

        if (context is null)
        {
            return Result.Failure<UploadContextResponse>(WeddingErrors.NotFound(new WeddingId(Guid.Empty)));
        }

        return context;
    }
}
