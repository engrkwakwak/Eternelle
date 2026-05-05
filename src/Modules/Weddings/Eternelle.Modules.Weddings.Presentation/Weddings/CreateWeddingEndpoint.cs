using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.CreateWedding;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class CreateWeddingEndpoint : IEndpoint
{
    internal sealed record Request(Guid TenantId, DateOnly WeddingDate, string? Hashtag);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings", async (Request request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateWeddingCommand(
                request.TenantId,
                request.WeddingDate,
                request.Hashtag);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Weddings);
    }
}
