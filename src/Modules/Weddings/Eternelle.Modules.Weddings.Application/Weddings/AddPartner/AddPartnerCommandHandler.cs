using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.AddPartner;

internal sealed class AddPartnerCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<AddPartnerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddPartnerCommand command, CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        Wedding? wedding = await weddingRepository.GetWithPartnersAsync(weddingId, cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<Guid>(WeddingErrors.NotFound(weddingId));
        }

        Result<Partner> result = wedding.AddPartner(
            (PartnerNumber)command.PartnerNumber,
            command.FirstName,
            command.LastName,
            command.Bio,
            command.ImageUrl,
            dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        weddingRepository.Update(wedding);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
