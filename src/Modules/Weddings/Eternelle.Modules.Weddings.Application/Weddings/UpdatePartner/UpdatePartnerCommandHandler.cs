using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdatePartner;

internal sealed class UpdatePartnerCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdatePartnerCommand>
{
    public async Task<Result> Handle(UpdatePartnerCommand command, CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        Wedding? wedding = await weddingRepository.GetWithPartnersAsync(weddingId, cancellationToken);

        if (wedding is null)
        {
            return Result.Failure(WeddingErrors.NotFound(weddingId));
        }

        Result result = wedding.UpdatePartner(
            new PartnerId(command.PartnerId),
            command.FirstName,
            command.LastName,
            command.Bio,
            command.ImageUrl,
            dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return result;
        }

        weddingRepository.Update(wedding);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
