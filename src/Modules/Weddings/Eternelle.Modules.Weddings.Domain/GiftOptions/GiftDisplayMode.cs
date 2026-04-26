namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

/// <summary>
/// Controls how a gift option card interacts when tapped.
///
///   Link          — opens link_url in a new tab (external registry, Honeyfund, etc.)
///   ModalDetails  — opens an in-site modal showing the QR code and account details
///   InlineDetails — expands the card in place to reveal account details
///
/// Couples that only want to accept bank/e-wallet transfers use ModalDetails or
/// InlineDetails with qr_image_url and account_* fields and no link_url. Couples
/// with an external registry use Link with just link_url. Mixed modes per wedding
/// are supported (e.g. one card per payment method, plus a registry card).
///
/// Maps to wedding.gift_display_mode PostgreSQL ENUM.
/// </summary>
public enum GiftDisplayMode
{
    Link = 1,
    ModalDetails = 2,
    InlineDetails = 3
}
