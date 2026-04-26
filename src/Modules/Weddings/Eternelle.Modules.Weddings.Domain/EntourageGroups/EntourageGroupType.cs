namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// Semantic classification for an entourage group. Lets the renderer apply
/// type-specific layouts without matching on label strings, and lets the domain
/// enforce which group types may contain couple pairings.
///
/// Null on EntourageGroup.GroupType means the group is fully custom / user-defined.
/// Use 'Other' when the group is generic but still explicitly classified.
///
/// Pairing rules (enforced by EntourageGroup.PairMembers):
///   Always pairs  — PrincipalSponsors, SecondarySponsors, Parents
///   May pair      — LittleOnes (little bride + little groom), Other, null (custom)
///   Never pairs   — Bridesmaids, Groomsmen, FlowerGirls, RingBearers, CoinBearers, BibleReaders
///
/// SecondarySponsors covers candle, veil, and cord sponsors — these are all secondary
/// sponsors in a Philippine Catholic wedding. The specific ceremony role is captured
/// in EntourageGroup.Label (e.g. "Candle Sponsors", "Veil &amp; Cord Sponsors").
///
/// Maps to wedding.entourage_group_type PostgreSQL ENUM.
/// </summary>
public enum EntourageGroupType
{
    Parents = 1,
    PrincipalSponsors = 2,

    /// <summary>
    /// Covers candle, veil, and cord sponsors. Always rendered as couples.
    /// Use EntourageGroup.Label to distinguish the specific ceremony role.
    /// </summary>
    SecondarySponsors = 3,

    Bridesmaids = 4,
    Groomsmen = 5,
    FlowerGirls = 6,
    RingBearers = 7,
    CoinBearers = 8,
    BibleReaders = 9,

    /// <summary>
    /// Little bride and little groom. May contain one couple pairing alongside solo members.
    /// </summary>
    LittleOnes = 10,

    Other = 11
}
