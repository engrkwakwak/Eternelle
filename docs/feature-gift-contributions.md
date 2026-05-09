# Feature: Gift Contribution Tracking

**Status: TODO — secondary feature, implement after all current Weddings module tasks are complete.**

Couples manually record who sent money or gifts, through which gift option, and whether a thank-you has been sent.

---

## Why this feature

Couples currently track received gifts in a spreadsheet or group chat. This feature brings that tracking into the dashboard alongside the gift guide, so the couple has a single place to see who gave, how much, and who still needs a thank-you message.

---

## Module placement

**Stay in the Weddings module.** A contribution is tightly scoped to a `GiftOption` on a specific wedding. No new module needed.

---

## Domain additions

### New aggregate: `GiftContribution`

```csharp
public sealed class GiftContribution : Entity
{
    public GiftContributionId Id           { get; private set; }
    public WeddingId          WeddingId    { get; private set; }
    public GiftOptionId       GiftOptionId { get; private set; }

    /// Cross-module reference to rsvp.guests — stored as plain UUID, never joined.
    /// Null when the contributor cannot be matched to a guest (anonymous envelope, unknown transfer).
    public Guid?    GuestId      { get; private set; }

    /// Free-text fallback when GuestId is null or when the name on the transfer differs.
    public string?  SenderName   { get; private set; }

    /// Optional — some couples prefer not to record amounts.
    public decimal? Amount       { get; private set; }
    public string?  Currency     { get; private set; }   // ISO 4217, e.g. "PHP"

    public string?  Note         { get; private set; }
    public DateTime ReceivedAt   { get; private set; }
    public bool     IsAcknowledged { get; private set; }

    public static GiftContribution Record(...) { }
    public void    SetAcknowledged(bool isAcknowledged) { }   // flips the flag both ways
    public void    Update(...)                 { }
}
```

---

## New table: `wedding.gift_contributions`

| Column           | Type          | Constraints                        | Notes |
|---|---|---|---|
| `id`             | `uuid`        | PK                                 | UUIDv7 |
| `wedding_id`     | `uuid`        | NOT NULL, FK → weddings            | Denormalized for direct queries |
| `gift_option_id` | `uuid`        | NOT NULL, FK → gift_options        | |
| `guest_id`       | `uuid`        | —                                  | Cross-module ref to rsvp.guests — no FK constraint |
| `sender_name`    | `text`        | —                                  | Free-text fallback |
| `amount`         | `numeric(12,2)` | —                                | Optional |
| `currency`       | `char(3)`     | —                                  | ISO 4217, defaults to "PHP" on the UI |
| `note`           | `text`        | —                                  | Couple's internal note |
| `received_at`    | `timestamptz` | NOT NULL                           | |
| `is_acknowledged`| `bool`        | NOT NULL, DEFAULT false            | Thank-you sent flag |

Index: `(wedding_id, gift_option_id)` — feeds the per-gift-option contribution list.
Index: `(wedding_id, is_acknowledged)` — feeds the pending thank-you queue.

### String constants

- `MaxSenderNameLength = 150`
- `MaxNoteLength = 500`
- `MaxCurrencyLength = 3`

---

## Commands

| Command | Actor | Notes |
|---|---|---|
| `RecordGiftContributionCommand(WeddingId, GiftOptionId, GuestId?, SenderName?, Amount?, Currency?, Note?, ReceivedAt)` | Couple (JWT) | Either `GuestId` or `SenderName` should be provided — validate at application layer (not domain invariant) |
| `UpdateGiftContributionCommand(ContributionId, SenderName?, Amount?, Currency?, Note?, ReceivedAt)` | Couple (JWT) | `GiftOptionId` is immutable after creation. `GuestId` is also immutable after creation — it is intentionally absent from `UpdateGiftContributionCommand` and cannot be changed post-record. |
| `DeleteGiftContributionCommand(ContributionId)` | Couple (JWT) | Hard delete |
| `SetAcknowledgedGiftContributionCommand(ContributionId, bool IsAcknowledged)` | Couple (JWT) | Flips `is_acknowledged` either way — reversible |

> **Future / deferred:** `BulkAcknowledgeGiftContributionsCommand(WeddingId, IReadOnlyList<ContributionId>)` — batch version for the thank-you queue UI. Not part of the initial implementation; add once the four core commands above are shipped.

> **Validation note — `RecordGiftContributionCommandValidator`:** at least one of `GuestId` or `SenderName` must be provided. Implement as a `Must` rule on `GuestId`:
>
> ```csharp
> RuleFor(c => c.GuestId)
>     .Must((cmd, guestId) => guestId.HasValue || !string.IsNullOrWhiteSpace(cmd.SenderName))
>     .WithMessage("Either GuestId or SenderName must be provided.");
> ```

---

## Queries

| Query | Notes |
|---|---|
| `GetGiftContributionsQuery(WeddingId, GiftOptionId?)` | All contributions for a wedding, optionally filtered by gift option; ordered by `received_at DESC` |
| `GetPendingAcknowledgmentsQuery(WeddingId)` | Contributions where `is_acknowledged = false`; thank-you queue |
| `GetGiftSummaryQuery(WeddingId)` | Totals per gift option (count, sum of amounts); dashboard summary card |

---

## Product decisions to resolve before implementing

- **Is `Amount` required?** Some couples only want to record who gave without showing money figures. If optional, the UI should make this clear.
- **Currency.** Philippines weddings are almost always PHP. Hard-code the default to "PHP" on the UI and treat it as an optional override field.
- **Export.** A CSV export (sender name, amount, gift type, received date, acknowledged) would be useful for thank-you card preparation. This is a presentation-layer feature — no domain changes needed, just a streaming endpoint.
- **Acknowledgment flow.** Decided: use `SetAcknowledged(bool isAcknowledged)` — flag is reversible, no state machine needed.

---

## Notes

- `guest_id` is a cross-module reference by value only — no FK constraint in the DB, no navigation in EF. The couple selects a guest from a dropdown on the client, which calls `GetGuestsQuery` from the RSVP module. If the RSVP module is not yet set up for a wedding, the couple falls back to free-text `sender_name`.
- No integration events needed — contribution tracking is entirely couple-internal.
- Bulk download / CSV export requires no new domain aggregate; the endpoint queries `gift_contributions` joined to `gift_options` and streams a ZIP or CSV directly.
