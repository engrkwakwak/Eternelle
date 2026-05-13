using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Storage;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GenerateUploadSlots;

internal sealed class GenerateUploadSlotsCommandHandler(
    IWeddingRepository weddingRepository,
    IGuestPhotoRepository guestPhotoRepository,
    IPhotoStorageService photoStorageService,
    IUploadSlotStore uploadSlotStore) : ICommandHandler<GenerateUploadSlotsCommand, IReadOnlyList<UploadSlotResponse>>
{
    public async Task<Result<IReadOnlyList<UploadSlotResponse>>> Handle(
        GenerateUploadSlotsCommand command,
        CancellationToken cancellationToken)
    {
        // Resolve wedding from token — same not-found pattern as the upload handler
        // so callers cannot probe token validity.
        Wedding? wedding = await weddingRepository.GetByUploadTokenAsync(
            command.UploadToken,
            cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<IReadOnlyList<UploadSlotResponse>>(
                WeddingErrors.NotFound(new WeddingId(Guid.Empty)));
        }

        // Check remaining capacity before generating slots.
        // Limits are derived from the wedding's plan tier — no extra DB round-trip needed.
        // A guest requesting 10 slots when only 3 remain gets a plan-limit error up front
        // rather than discovering it during registration.
        int? planLimit = WeddingPlanLimits.PhotoLimit(wedding.Plan);

        if (planLimit is not null)
        {
            int current = await guestPhotoRepository.CountByWeddingIdAsync(
                wedding.Id, cancellationToken);

            if (current + command.Count > planLimit.Value)
            {
                return Result.Failure<IReadOnlyList<UploadSlotResponse>>(
                    GuestPhotoErrors.PlanLimitReached);
            }
        }

        // Generate presigned slots from the CDN provider and persist them in the slot store.
        IReadOnlyList<PresignedUploadSlot> slots =
            await photoStorageService.GeneratePresignedSlotsAsync(command.Count, cancellationToken);

        foreach (PresignedUploadSlot slot in slots)
        {
            await uploadSlotStore.StoreAsync(slot.SlotId, slot.CdnUrl, cancellationToken);
        }

        IReadOnlyList<UploadSlotResponse> response = slots
            .Select(s => new UploadSlotResponse(s.SlotId, s.PresignedUrl, s.CdnUrl))
            .ToList();

        return Result.Success(response);
    }
}
