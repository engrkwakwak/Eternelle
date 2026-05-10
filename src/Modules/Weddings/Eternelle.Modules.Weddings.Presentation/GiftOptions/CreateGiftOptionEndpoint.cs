using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GiftOptions.CreateGiftOption;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GiftOptions;

internal sealed class CreateGiftOptionEndpoint : IEndpoint
{
    internal sealed record Request(
        string Title,
        string? Description,
        int DisplayMode,
        string? LinkUrl,
        string? ImageUrl,
        string? QrImageUrl,
        string? AccountName,
        string? AccountNumber,
        string? AccountType,
        int DisplayOrder);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/gift-options", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateGiftOptionCommand(
                weddingId,
                request.Title,
                request.Description,
                (GiftDisplayMode)request.DisplayMode,
                request.LinkUrl,
                request.ImageUrl,
                request.QrImageUrl,
                request.AccountName,
                request.AccountNumber,
                request.AccountType,
                request.DisplayOrder);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/gift-options/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.GiftOptions)
        .RequireAuthorization("wedding:edit");
    }
}
