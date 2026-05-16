using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Common.Domain.ValueObjects;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdatePartner;

internal sealed class UpdatePartnerCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdatePartnerCommand>
{
    public async Task<Result> Handle(UpdatePartnerCommand command, CancellationToken cancellationToken)
    {
        Result<PersonFirstName> firstNameResult = PersonFirstName.Create(command.FirstName);
        if (firstNameResult.IsFailure)
        {
            return Result.Failure(firstNameResult.Error);
        }

        Result<PersonLastName> lastNameResult = PersonLastName.Create(command.LastName);
        if (lastNameResult.IsFailure)
        {
            return Result.Failure(lastNameResult.Error);
        }

        Biography? bio = null;
        if (command.Bio is not null)
        {
            Result<Biography> bioResult = Biography.Create(command.Bio);
            if (bioResult.IsFailure)
            {
                return Result.Failure(bioResult.Error);
            }
            bio = bioResult.Value;
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
        }

        var weddingId = new WeddingId(command.WeddingId);

        Wedding? wedding = await weddingRepository.GetWithPartnersAsync(weddingId, cancellationToken);

        if (wedding is null)
        {
            return Result.Failure(WeddingErrors.NotFound(weddingId));
        }

        Result result = wedding.UpdatePartner(
            new PartnerId(command.PartnerId),
            firstNameResult.Value,
            lastNameResult.Value,
            bio,
            imageUrl,
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
