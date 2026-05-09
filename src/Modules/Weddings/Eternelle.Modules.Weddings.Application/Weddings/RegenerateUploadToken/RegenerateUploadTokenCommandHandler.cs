using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.RegenerateUploadToken;

internal sealed class RegenerateUploadTokenCommandHandler(
    IWeddingRepository weddingRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<RegenerateUploadTokenCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegenerateUploadTokenCommand command, CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        Wedding? wedding = await weddingRepository.GetWithSnapShareAsync(weddingId, cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<Guid>(WeddingErrors.NotFound(weddingId));
        }

        Result result = wedding.RegenerateUploadToken(dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        weddingRepository.Update(wedding);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(wedding.SnapShare!.UploadToken!.Value);
    }
}
