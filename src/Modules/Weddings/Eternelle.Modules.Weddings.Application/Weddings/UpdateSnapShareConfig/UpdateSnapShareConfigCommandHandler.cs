using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateSnapShareConfig;

internal sealed class UpdateSnapShareConfigCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdateSnapShareConfigCommand>
{
    public async Task<Result> Handle(UpdateSnapShareConfigCommand command, CancellationToken cancellationToken)
    {
        Wedding? wedding = await weddingRepository.GetAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        if (wedding is null)
        {
            return Result.Failure(WeddingErrors.NotFound(new WeddingId(command.WeddingId)));
        }

        InstagramHandle? instagramHandle = null;
        if (command.InstagramHandle is not null)
        {
            Result<InstagramHandle> handleResult = InstagramHandle.Create(command.InstagramHandle);
            if (handleResult.IsFailure)
            {
                return Result.Failure(handleResult.Error);
            }

            instagramHandle = handleResult.Value;
        }

        wedding.ConfigureSnapShare(
            instagramHandle,
            command.CtaText,
            command.Enabled,
            command.ModerationMode,
            dateTimeProvider.UtcNow);

        weddingRepository.Update(wedding);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

