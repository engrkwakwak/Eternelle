using Eternelle.Common.Domain;
using Eternelle.Common.Domain.ValueObjects;
using Eternelle.Modules.Weddings.Domain.Shared;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// An entity within the Wedding aggregate. A wedding always has exactly two partners
/// (PartnerNumber.One and PartnerNumber.Two). Partner is never accessed directly —
/// always through the Wedding aggregate root.
/// </summary>
public sealed class Partner : Entity
{
    private Partner()
    {
    }

    public PartnerId Id { get; private set; }

    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// Identifies which of the two partners this is.
    /// The enum enforces the closed set at the type level — no runtime guard needed.
    /// </summary>
    public PartnerNumber PartnerNumber { get; private set; }

    public PersonFirstName FirstName { get; private set; }

    public PersonLastName LastName { get; private set; }

    public Biography? Bio { get; private set; }

    public ImageUrl? ImageUrl { get; private set; }

    internal static Partner Create(
        WeddingId weddingId,
        PartnerNumber partnerNumber,
        PersonFirstName firstName,
        PersonLastName lastName,
        Biography? bio,
        ImageUrl? imageUrl)
    {
        return new Partner
        {
            Id = PartnerId.New(),
            WeddingId = weddingId,
            PartnerNumber = partnerNumber,
            FirstName = firstName,
            LastName = lastName,
            Bio = bio,
            ImageUrl = imageUrl
        };
    }

    internal void Update(PersonFirstName firstName, PersonLastName lastName, Biography? bio, ImageUrl? imageUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        ImageUrl = imageUrl;
    }
}
