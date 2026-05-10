using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GiftOptions.UpdateGiftOption;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GiftOptions;

internal sealed class UpdateGiftOptionEndpoint : IEndpoint
{
    internal sealed record Request(
        string Title,
        string? Description,
        GiftDisplayMode DisplayMode,
        string? LinkUrl,
        string? ImageUrl,
        string? QrImageUrl,
        string? AccountName,
        string? AccountNumber,
        string? AccountType);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/gift-options/{id}", async (
            Guid weddingId,
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateGiftOptionCommand(
                weddingId,
                id,
                request.Title,
                request.Description,
                request.DisplayMode,
                request.LinkUrl,
                request.ImageUrl,
                request.QrImageUrl,
                request.AccountName,
                request.AccountNumber,
                request.AccountType);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.GiftOptions)
        .RequireAuthorization("wedding:edit");
    }
}
