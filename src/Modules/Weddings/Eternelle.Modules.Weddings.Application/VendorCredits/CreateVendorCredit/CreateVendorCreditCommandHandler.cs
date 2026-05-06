using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.CreateVendorCredit;

internal sealed class CreateVendorCreditCommandHandler(
    IVendorCreditRepository vendorCreditRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVendorCreditCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateVendorCreditCommand command,
        CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        IReadOnlyList<VendorCredit> existingCredits = await vendorCreditRepository.GetByWeddingIdAsync(
            weddingId,
            cancellationToken);

        int displayOrder = existingCredits.Count == 0
            ? 0
            : existingCredits.Max(c => c.DisplayOrder) + 1;

        InstagramHandle? instagramHandle = null;
        if (!string.IsNullOrWhiteSpace(command.InstagramHandle))
        {
            Result<InstagramHandle> handleResult = InstagramHandle.Create(command.InstagramHandle);
            if (handleResult.IsFailure)
            {
                return Result.Failure<Guid>(handleResult.Error);
            }
            instagramHandle = handleResult.Value;
        }

        var credit = VendorCredit.Create(
            weddingId,
            command.Name,
            command.Role,
            command.WebsiteUrl,
            command.ImageUrl,
            instagramHandle,
            displayOrder);

        vendorCreditRepository.Insert(credit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(credit.Id.Value);
    }
}
