using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.RemoveGalleryImage;

public sealed record RemoveGalleryImageCommand(Guid GalleryImageId) : ICommand;
