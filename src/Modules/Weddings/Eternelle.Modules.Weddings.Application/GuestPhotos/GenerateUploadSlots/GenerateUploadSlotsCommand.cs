using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GenerateUploadSlots;

public sealed record GenerateUploadSlotsCommand(Guid UploadToken, int Count)
    : ICommand<IReadOnlyList<UploadSlotResponse>>
{
    public const int MaxBatchSize = 30;
}
