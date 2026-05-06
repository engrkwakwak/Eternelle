using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.ReorderVendorCredits;

internal sealed class ReorderVendorCreditsCommandHandler(
    IVendorCreditRepository vendorCreditRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderVendorCreditsCommand>
{
    public async Task<Result> Handle(
        ReorderVendorCreditsCommand command,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<VendorCredit> credits = await vendorCreditRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var creditsById = credits.ToDictionary(c => c.Id.Value);

        HashSet<Guid> providedIds = [.. command.VendorCreditIds];

        if (command.VendorCreditIds.Count != providedIds.Count ||
            providedIds.Count != credits.Count ||
            !providedIds.SetEquals(creditsById.Keys))
        {
            return Result.Failure(VendorCreditErrors.ReorderListMismatch);
        }

        for (int i = 0; i < command.VendorCreditIds.Count; i++)
        {
            VendorCredit credit = creditsById[command.VendorCreditIds[i]];
            credit.Reorder(i);
            vendorCreditRepository.Update(credit);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
