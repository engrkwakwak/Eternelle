using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

internal sealed class GetWeddingQueryHandler(
    IWeddingRepository weddingRepository) : IQueryHandler<GetWeddingQuery, WeddingResponse>
{
    public async Task<Result<WeddingResponse>> Handle(GetWeddingQuery query, CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(query.WeddingId);

        Wedding? wedding = await weddingRepository.GetWithPartnersAsync(weddingId, cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<WeddingResponse>(WeddingErrors.NotFound(weddingId));
        }

        return MapToResponse(wedding);
    }

    private static WeddingResponse MapToResponse(Wedding wedding)
    {
        IReadOnlyList<PartnerResponse> partners = wedding.Partners
            .Select(p => new PartnerResponse(
                p.Id.Value,
                (int)p.PartnerNumber,
                p.FirstName,
                p.LastName,
                p.Bio,
                p.ImageUrl))
            .ToList();

        SnapShareResponse? snapShare = wedding.SnapShare is null
            ? null
            : new SnapShareResponse(
                wedding.SnapShare.Id.Value,
                wedding.SnapShare.InstagramHandle?.Value,
                wedding.SnapShare.CtaText,
                wedding.SnapShare.Enabled);

        return new WeddingResponse(
            wedding.Id.Value,
            wedding.TenantId,
            wedding.WeddingDate,
            wedding.Hashtag?.Value,
            wedding.CreatedAtUtc,
            wedding.UpdatedAtUtc,
            partners,
            snapShare);
    }
}
