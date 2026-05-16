using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.AddPartner;

internal sealed class AddPartnerCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<AddPartnerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddPartnerCommand command, CancellationToken cancellationToken)
    {
        Result<PersonFirstName> firstNameResult = PersonFirstName.Create(command.FirstName);
        if (firstNameResult.IsFailure)
        {
            return Result.Failure<Guid>(firstNameResult.Error);
        }

        Result<PersonLastName> lastNameResult = PersonLastName.Create(command.LastName);
        if (lastNameResult.IsFailure)
        {
            return Result.Failure<Guid>(lastNameResult.Error);
        }

        Biography? bio = null;
        if (command.Bio is not null)
        {
            Result<Biography> bioResult = Biography.Create(command.Bio);
            if (bioResult.IsFailure)
            {
                return Result.Failure<Guid>(bioResult.Error);
            }
            bio = bioResult.Value;
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure<Guid>(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
        }

        var weddingId = new WeddingId(command.WeddingId);

        Wedding? wedding = await weddingRepository.GetWithPartnersAsync(weddingId, cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<Guid>(WeddingErrors.NotFound(weddingId));
        }

        Result<Partner> result = wedding.AddPartner(
            (PartnerNumber)command.PartnerNumber,
            firstNameResult.Value,
            lastNameResult.Value,
            bio,
            imageUrl,
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
