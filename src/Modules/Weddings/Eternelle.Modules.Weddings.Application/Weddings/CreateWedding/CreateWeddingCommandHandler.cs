using Eternelle.Common.Application.Clock;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings.CreateWedding;

internal sealed class CreateWeddingCommandHandler(
    IWeddingRepository weddingRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<CreateWeddingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateWeddingCommand command, CancellationToken cancellationToken)
    {
        Wedding? existing = await weddingRepository.GetByTenantIdAsync(command.TenantId, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<Guid>(WeddingErrors.TenantAlreadyHasWedding(command.TenantId));
        }

        Hashtag? hashtag = null;
        if (command.Hashtag is not null)
        {
            Result<Hashtag> hashtagResult = Hashtag.Create(command.Hashtag);
            if (hashtagResult.IsFailure)
            {
                return Result.Failure<Guid>(hashtagResult.Error);
            }

            hashtag = hashtagResult.Value;
        }

        var wedding = Wedding.Create(
            command.TenantId,
            command.WeddingDate,
            hashtag,
            dateTimeProvider.UtcNow);

        weddingRepository.Insert(wedding);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return wedding.Id.Value;
    }
}
