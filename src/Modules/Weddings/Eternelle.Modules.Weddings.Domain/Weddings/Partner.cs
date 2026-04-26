using Eternelle.Common.Domain;

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

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string? Bio { get; private set; }

    public string? ImageUrl { get; private set; }

    internal static Partner Create(
        WeddingId weddingId,
        PartnerNumber partnerNumber,
        string firstName,
        string lastName,
        string? bio,
        string? imageUrl)
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

    internal void Update(string firstName, string lastName, string? bio, string? imageUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        ImageUrl = imageUrl;
    }
}
