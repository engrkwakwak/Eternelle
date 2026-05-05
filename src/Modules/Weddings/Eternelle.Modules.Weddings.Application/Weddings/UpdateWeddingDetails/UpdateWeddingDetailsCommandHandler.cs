using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateWeddingDetails;

internal sealed class UpdateWeddingDetailsCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdateWeddingDetailsCommand>
{
    public async Task<Result> Handle(UpdateWeddingDetailsCommand command, CancellationToken cancellationToken)
    {
        Wedding? wedding = await weddingRepository.GetAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        if (wedding is null)
        {
            return Result.Failure(WeddingErrors.NotFound(new WeddingId(command.WeddingId)));
        }

        Hashtag? hashtag = null;
        if (command.Hashtag is not null)
        {
            Result<Hashtag> hashtagResult = Hashtag.Create(command.Hashtag);
            if (hashtagResult.IsFailure)
            {
                return Result.Failure(hashtagResult.Error);
            }

            hashtag = hashtagResult.Value;
        }

        Result result = wedding.UpdateDetails(
            command.WeddingDate,
            hashtag,
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
