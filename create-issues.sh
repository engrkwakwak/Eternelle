#!/usr/bin/env bash
# =============================================================================
# Eternelle — GitHub Issues Bootstrap
# Creates: 6 labels · 6 milestones · ~90 issues
#
# Usage:
#   GITHUB_TOKEN=ghp_xxxx GITHUB_REPO=yourname/eternelle bash create-issues.sh
#
# Requires: curl, node  (node is used for JSON encoding)
# =============================================================================
set -euo pipefail

TOKEN="${GITHUB_TOKEN:?Set GITHUB_TOKEN — github.com/settings/tokens (repo scope)}"
REPO="${GITHUB_REPO:?Set GITHUB_REPO as owner/repo, e.g. carlmarion/eternelle}"
API="https://api.github.com/repos/${REPO}"

# ─── Helpers ─────────────────────────────────────────────────────────────────

# JSON-encode a shell string (pass as argv — avoids stdin buffering issues in Git Bash)
jstr() { node -e "process.stdout.write(JSON.stringify(process.argv[1]))" -- "$1"; }

# Portable curl wrapper: writes body to tmp file, returns "BODY\nHTTP_CODE"
gh_req() {
  local method="$1" path="$2" data="${3:-}"
  local tmp; tmp=$(mktemp)
  local code
  if [[ -n "$data" ]]; then
    code=$(curl -s -o "$tmp" -w "%{http_code}" -X "$method" "${API}${path}" \
      -H "Authorization: Bearer ${TOKEN}" \
      -H "Accept: application/vnd.github+json" \
      -H "Content-Type: application/json" \
      -d "$data")
  else
    code=$(curl -s -o "$tmp" -w "%{http_code}" -X "$method" "${API}${path}" \
      -H "Authorization: Bearer ${TOKEN}" \
      -H "Accept: application/vnd.github+json")
  fi
  local body; body=$(cat "$tmp"); rm -f "$tmp"
  printf '%s\n%s' "$body" "$code"
}

create_label() {
  local name="$1" color="$2" desc="${3:-}"
  printf '  %-22s' "$name"
  local resp; resp=$(gh_req POST /labels \
    "{\"name\":$(jstr "$name"),\"color\":\"$color\",\"description\":$(jstr "$desc")}")
  local code; code=$(printf '%s' "$resp" | tail -1)
  case "$code" in
    201) echo "created" ;;
    422) echo "already exists (skip)" ;;
    *)   echo "HTTP $code — check token/permissions" ;;
  esac
}

create_milestone() {
  local title="$1" desc="${2:-}"
  printf '  %-35s' "$title"
  local resp; resp=$(gh_req POST /milestones \
    "{\"title\":$(jstr "$title"),\"description\":$(jstr "$desc")}")
  local code; code=$(printf '%s' "$resp" | tail -1)
  local body; body=$(printf '%s' "$resp" | head -n -1)
  if [[ "$code" == "201" ]]; then
    local num; num=$(printf '%s' "$body" | node -e "let d='';process.stdin.on('data',c=>d+=c);process.stdin.on('end',()=>process.stdout.write(String(JSON.parse(d).number)))")
    echo "created (#$num)"
    echo "$num"
  elif [[ "$code" == "422" ]]; then
    # Already exists — fetch the existing milestone number
    local existing
    existing=$(gh_req GET "/milestones?state=open&per_page=100" \
      | head -n -1 \
      | node -e "let d='';process.stdin.on('data',c=>d+=c);process.stdin.on('end',()=>{const ms=JSON.parse(d),m=ms.find(x=>x.title===process.argv[1]);process.stdout.write(m?String(m.number):'0')})" "$title")
    echo "already exists (#$existing)"
    echo "$existing"
  else
    echo "ERROR HTTP $code" >&2; echo "0"
  fi
}

# create_issue TITLE MILESTONE_NUM BODY label [label ...]
create_issue() {
  local title="$1" milestone="$2" body="$3"
  shift 3
  printf '    → %s\n' "$title"

  # Write title + body to temp files — avoids Git Bash mangling multi-line
  # strings and Unicode (em dashes, backticks) when passed through shell vars.
  local tmp_t tmp_b tmp_l tmp_p
  tmp_t=$(mktemp); printf '%s' "$title" > "$tmp_t"
  tmp_b=$(mktemp); printf '%s' "$body"  > "$tmp_b"
  tmp_l=$(mktemp); node -e "process.stdout.write(JSON.stringify(process.argv.slice(1)))" -- "$@" > "$tmp_l"
  tmp_p=$(mktemp)

  # Build complete JSON payload using node (reads from files, no shell quoting)
  node -e "
    const fs = require('fs');
    const payload = {
      title:     fs.readFileSync(process.argv[1], 'utf8'),
      body:      fs.readFileSync(process.argv[2], 'utf8'),
      milestone: parseInt(process.argv[3]),
      labels:    JSON.parse(fs.readFileSync(process.argv[4], 'utf8'))
    };
    fs.writeFileSync(process.argv[5], JSON.stringify(payload));
  " "$tmp_t" "$tmp_b" "$milestone" "$tmp_l" "$tmp_p"

  rm -f "$tmp_t" "$tmp_b" "$tmp_l"

  # POST payload directly from file
  local resp code
  resp=$(curl -s -w '\n%{http_code}' -X POST "${API}/issues" \
    -H "Authorization: Bearer ${TOKEN}" \
    -H "Accept: application/vnd.github+json" \
    -H "Content-Type: application/json" \
    --data-binary "@${tmp_p}")
  code=$(printf '%s' "$resp" | tail -1)
  rm -f "$tmp_p"

  if [[ "$code" != "201" ]]; then
    local msg; msg=$(printf '%s' "$resp" | head -n -1 | \
      node -e "let d='';process.stdin.on('data',c=>d+=c);process.stdin.on('end',()=>{try{process.stdout.write(JSON.parse(d).message)}catch(e){process.stdout.write(d)}})")
    echo "      ✗ FAILED (HTTP $code) — ${msg}"
  fi
  sleep 0.8   # Stay under GitHub's secondary rate limit (~80 req/min)
}

# =============================================================================
echo "━━━ LABELS ━━━"
create_label "domain"         "0075ca" "Aggregates, entities, value objects, domain events"
create_label "application"    "0e8a16" "Commands, queries, validators, handlers"
create_label "infrastructure" "e4e669" "EF Core, repositories, migrations, outbox/inbox"
create_label "api"            "d93f0b" "Minimal API endpoints"
create_label "testing"        "cfd3d7" "Unit, integration, architecture tests"
create_label "cross-module"   "f9d0c4" "Integration events and inter-module contracts"

# =============================================================================
echo ""
echo "━━━ MILESTONES ━━━"

# tail -1 extracts just the milestone number from create_milestone output
MS_WEDDINGS=$(create_milestone "Weddings Module" \
  "Wedding content CRUD: profiles, partners, story, entourage, gallery, gifts, dress code, ceremony acts, vendor credits, reminders, snap share" \
  | tail -1)

MS_RSVP=$(create_milestone "RSVP Module" \
  "Guest list management, RSVP submissions, day-of check-in, QR code generation" \
  | tail -1)

MS_GUESTBOOK=$(create_milestone "Guestbook Module" \
  "Public guestbook entry submission and couple moderation" \
  | tail -1)

MS_IAM=$(create_milestone "IAM Module" \
  "Shadow user profiles (JIT provisioning from Keycloak JWT), RBAC roles and permissions" \
  | tail -1)

MS_TENANCY=$(create_milestone "Tenancy Module" \
  "Tenant lifecycle, hostname resolution, template assignment, section ordering" \
  | tail -1)

MS_CATALOG=$(create_milestone "Catalog Module" \
  "Template registry, theme definitions, section variants" \
  | tail -1)

echo ""
echo "Milestone IDs captured:"
echo "  Weddings=$MS_WEDDINGS  RSVP=$MS_RSVP  Guestbook=$MS_GUESTBOOK"
echo "  IAM=$MS_IAM  Tenancy=$MS_TENANCY  Catalog=$MS_CATALOG"
if [[ "$MS_WEDDINGS" == "0" || -z "$MS_WEDDINGS" ]]; then
  echo "ERROR: Milestone ID capture failed. Check your token has 'repo' scope." >&2
  exit 1
fi

# =============================================================================
echo ""
echo "━━━ ISSUES — Weddings Module ━━━"

# ── Application ───────────────────────────────────────────────────────────────

create_issue \
  "[App] Wedding profile commands — UpdateDetails, UpdateSnapShare" \
  "$MS_WEDDINGS" \
  "## Scope
Application-layer write operations for the core \`Wedding\` aggregate.

### Commands
- \`UpdateWeddingDetailsCommand\` — fields: \`weddingDate\`, \`hashtag\`, \`livestreamUrl\`, \`message\`
- \`UpdateSnapShareConfigCommand\` — fields: \`instagramHandle\`, \`ctaText\`, \`enabled\`

### Each command needs
- Command record
- \`ICommandHandler<TCommand, Result>\` handler — load wedding from repo, call domain method, SaveChanges
- FluentValidation validator
- Raises existing domain events: \`WeddingDetailsUpdatedDomainEvent\` / \`WeddingSnapShareUpdatedDomainEvent\`

### Reference
Pattern: \`src/Modules/Weddings/.../Weddings/CreateWedding/\`" \
  "application"

create_issue \
  "[App] Wedding profile query — GetWedding" \
  "$MS_WEDDINGS" \
  "## Scope
Query returning full wedding details including partners.

### Files
- \`GetWeddingQuery(WeddingId weddingId)\`
- \`GetWeddingQueryHandler\` → \`Result<WeddingResponse>\`
- \`WeddingResponse\` DTO: all profile fields + \`IReadOnlyList<PartnerResponse>\`

### Note
Uses \`IWeddingRepository.GetWithPartnersAsync()\`. Already exists in infrastructure — just wire the handler." \
  "application"

create_issue \
  "[App] Partner commands — AddPartner, UpdatePartner" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for managing the two owned \`Partner\` entities on the \`Wedding\` aggregate.

### Commands
- \`AddPartnerCommand(WeddingId, PartnerNumber, FirstName, LastName, Bio?, ImageUrl?)\`
- \`UpdatePartnerCommand(WeddingId, PartnerId, FirstName, LastName, Bio?, ImageUrl?)\`

### Notes
- Partners are owned entities on Wedding, not separate aggregate roots
- \`PartnerNumber\` must be 1 or 2 — validate in FluentValidation and at the domain level
- Handler loads Wedding, calls \`wedding.AddPartner()\` / \`wedding.UpdatePartner()\`, SaveChanges" \
  "application"

create_issue \
  "[App] StoryMoment commands — Create, Update, Delete, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Write operations for the \`StoryMoment\` aggregate.

### Commands
- \`CreateStoryMomentCommand(WeddingId, Title, StoryDate?, Description, ImageUrl?, DisplayOrder)\`
- \`UpdateStoryMomentCommand(StoryMomentId, Title, StoryDate?, Description, ImageUrl?)\`
- \`DeleteStoryMomentCommand(StoryMomentId)\`
- \`ReorderStoryMomentsCommand(WeddingId, IReadOnlyList<StoryMomentId>)\` — sets \`DisplayOrder\` from list index

### DB fields (wedding.story_moments)
\`title text NOT NULL, story_date date, description text NOT NULL, image_url text, display_order int NOT NULL\`" \
  "application"

create_issue \
  "[App] StoryMoment query — GetStoryMoments" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetStoryMomentsQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<StoryMomentResponse>>\`
- Ordered by \`display_order ASC\`" \
  "application"

create_issue \
  "[App] EntourageGroup commands — Create, Update, Delete, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`EntourageGroup\` aggregate root.

### Commands
- \`CreateEntourageGroupCommand(WeddingId, Label, Subtitle?, GroupType?, RenderAs, DisplayOrder)\`
- \`UpdateEntourageGroupCommand(EntourageGroupId, Label, Subtitle?, GroupType?, RenderAs)\`
- \`DeleteEntourageGroupCommand(EntourageGroupId)\` — cascades to members and couples
- \`ReorderEntourageGroupsCommand(WeddingId, IReadOnlyList<EntourageGroupId>)\`

### Enums
- \`GroupType\` (nullable): parents, principal_sponsors, secondary_sponsors, bridesmaids, groomsmen, flower_girls, ring_bearers, other
- \`RenderAs\`: cards | list" \
  "application"

create_issue \
  "[App] EntourageMember commands — Add, Update, Remove, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for \`EntourageMember\` child entities within an \`EntourageGroup\`.

### Commands
- \`AddEntourageMemberCommand(GroupId, Name, Role, ImageUrl?, Message?, Note?, Seed?)\`
- \`UpdateEntourageMemberCommand(MemberId, Name, Role, ImageUrl?, Message?, Note?, Seed?)\`
- \`RemoveEntourageMemberCommand(MemberId)\` — also removes any EntourageCouple rows referencing this member
- \`ReorderEntourageMembersCommand(GroupId, IReadOnlyList<EntourageMemberId>)\`

### DB constraint
Every member MUST belong to a group — \`group_id\` is NOT NULL. No ungrouped member concept." \
  "application"

create_issue \
  "[App] EntourageCouple commands — Pair, Remove" \
  "$MS_WEDDINGS" \
  "## Scope
Commands to pair two entourage members into a rendered couple (e.g. principal sponsors).

### Commands
- \`PairEntourageCoupleCommand(GroupId, MemberAId, MemberBId, Note?)\`
- \`RemoveEntourageCoupleCommand(CoupleId)\`

### Domain constraint
DB enforces \`CHECK (member_a_id < member_b_id)\` (UUID ordering) to prevent duplicate pairs with swapped slots. Handler must sort the two IDs before calling the domain method: if \`MemberAId > MemberBId\` swap them." \
  "application"

create_issue \
  "[App] EntourageGroup query — GetEntourageGroups" \
  "$MS_WEDDINGS" \
  "## Scope
Returns the full entourage tree for a wedding — groups with nested members and couples.

### Query
- \`GetEntourageGroupsQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<EntourageGroupResponse>>\`
- Each group: \`Members\` list (ordered by display_order) + \`Couples\` list

### Note
Single repository call loading the full tree. This is a public-facing read endpoint so performance matters — consider a Dapper projection query rather than EF eager loading for the full tree." \
  "application"

create_issue \
  "[App] GalleryImage commands — Add, Update, Remove, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`GalleryImage\` aggregate.

### Commands
- \`AddGalleryImageCommand(WeddingId, SrcUrl, AltText, WidthPx?, HeightPx?, Caption?)\`
- \`UpdateGalleryImageCommand(GalleryImageId, SrcUrl, AltText, WidthPx?, HeightPx?, Caption?)\`
- \`RemoveGalleryImageCommand(GalleryImageId)\`
- \`ReorderGalleryImagesCommand(WeddingId, IReadOnlyList<GalleryImageId>)\`

### DB fields (wedding.gallery_images)
\`src_url text NOT NULL, alt_text text NOT NULL, width_px int, height_px int, caption text, display_order int NOT NULL\`" \
  "application"

create_issue \
  "[App] GalleryImage query — GetGalleryImages" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetGalleryImagesQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<GalleryImageResponse>>\`
- Ordered by \`display_order ASC\`" \
  "application"

create_issue \
  "[App] GiftOption commands — Create, Update, Delete, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`GiftOption\` aggregate.

### Commands
- \`CreateGiftOptionCommand(WeddingId, Title, Description?, DisplayMode, LinkUrl?, ImageUrl?, QrImageUrl?, AccountName?, AccountNumber?, AccountType?)\`
- \`UpdateGiftOptionCommand(GiftOptionId, ...same fields...)\`
- \`DeleteGiftOptionCommand(GiftOptionId)\`
- \`ReorderGiftOptionsCommand(WeddingId, IReadOnlyList<GiftOptionId>)\`

### Enum
\`DisplayMode\`: link | modal_details | inline_details

### Validation rule
If \`DisplayMode == link\` then \`LinkUrl\` must not be null/empty. Add this cross-field rule in the FluentValidation validator." \
  "application"

create_issue \
  "[App] GiftOption query — GetGiftOptions" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetGiftOptionsQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<GiftOptionResponse>>\`
- Ordered by \`display_order ASC\`" \
  "application"

create_issue \
  "[App] DressCodeConfig commands — Create/Update config, manage colors and images" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`DressCodeConfig\` aggregate (1:1 with Wedding) and its child collections.

### Config commands
- \`CreateDressCodeConfigCommand(WeddingId, Description)\`
- \`UpdateDressCodeConfigCommand(DressCodeConfigId, Description)\`

### Color commands
- \`AddDressCodeColorCommand(DressCodeConfigId, ColorHex, ColorName)\`
- \`RemoveDressCodeColorCommand(DressCodeColorId)\`
- \`ReorderDressCodeColorsCommand(DressCodeConfigId, IReadOnlyList<DressCodeColorId>)\`

### Image commands
- \`AddDressCodeImageCommand(DressCodeConfigId, ImageUrl)\`
- \`RemoveDressCodeImageCommand(DressCodeImageId)\`
- \`ReorderDressCodeImagesCommand(DressCodeConfigId, IReadOnlyList<DressCodeImageId>)\`

### Domain note
Hex validation is enforced by the \`HexColor\` value object in domain. The validator should call \`HexColor.Create(colorHex)\` and return its error on failure — do NOT duplicate the regex in the validator." \
  "application"

create_issue \
  "[App] DressCodeConfig query — GetDressCodeConfig" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetDressCodeConfigQuery(WeddingId)\`
- Returns \`Result<DressCodeConfigResponse>\`
- Response: description + \`Colors\` list (ordered by display_order) + \`Images\` list (ordered by display_order)" \
  "application"

create_issue \
  "[App] CeremonyAct commands — Create, Update, Delete, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`CeremonyAct\` aggregate.

### Commands
- \`CreateCeremonyActCommand(WeddingId, Name, Description?, Icon?, ActTime?)\`
- \`UpdateCeremonyActCommand(CeremonyActId, Name, Description?, Icon?, ActTime?)\`
- \`DeleteCeremonyActCommand(CeremonyActId)\`
- \`ReorderCeremonyActsCommand(WeddingId, IReadOnlyList<CeremonyActId>)\`

### DB fields (wedding.ceremony_acts)
\`name text NOT NULL, description text, icon text, act_time time, display_order int NOT NULL\`" \
  "application"

create_issue \
  "[App] CeremonyAct query — GetCeremonyActs" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetCeremonyActsQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<CeremonyActResponse>>\`
- Ordered by \`display_order ASC\`" \
  "application"

create_issue \
  "[App] VendorCredit commands — Create, Update, Delete, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`VendorCredit\` aggregate.

### Commands
- \`CreateVendorCreditCommand(WeddingId, Name, Role, WebsiteUrl?, ImageUrl?, InstagramHandle?)\`
- \`UpdateVendorCreditCommand(VendorCreditId, Name, Role, WebsiteUrl?, ImageUrl?, InstagramHandle?)\`
- \`DeleteVendorCreditCommand(VendorCreditId)\`
- \`ReorderVendorCreditsCommand(WeddingId, IReadOnlyList<VendorCreditId>)\`

### DB fields (wedding.vendor_credits)
\`name text NOT NULL, role text NOT NULL, website_url text, image_url text, instagram_handle text, display_order int NOT NULL\`" \
  "application"

create_issue \
  "[App] VendorCredit query — GetVendorCredits" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetVendorCreditsQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<VendorCreditResponse>>\`
- Ordered by \`display_order ASC\`" \
  "application"

create_issue \
  "[App] Reminder commands — Create, Update, Delete, Reorder" \
  "$MS_WEDDINGS" \
  "## Scope
Commands for the \`Reminder\` aggregate (first-class tier feature).

### Commands
- \`CreateReminderCommand(WeddingId, Icon, Title, Body)\`
- \`UpdateReminderCommand(ReminderId, Icon, Title, Body)\`
- \`DeleteReminderCommand(ReminderId)\`
- \`ReorderRemindersCommand(WeddingId, IReadOnlyList<ReminderId>)\`

### DB fields (wedding.reminders)
\`icon text NOT NULL, title text NOT NULL, body text NOT NULL, display_order int NOT NULL\`" \
  "application"

create_issue \
  "[App] Reminder query — GetReminders" \
  "$MS_WEDDINGS" \
  "## Scope
- \`GetRemindersQuery(WeddingId)\`
- Returns \`Result<IReadOnlyList<ReminderResponse>>\`
- Ordered by \`display_order ASC\`" \
  "application"

# ── Infrastructure ────────────────────────────────────────────────────────────

create_issue \
  "[Infra] EF Core configs — EntourageGroup, EntourageMember, EntourageCouple" \
  "$MS_WEDDINGS" \
  "## Scope
\`IEntityTypeConfiguration<T>\` implementations for the entourage aggregate hierarchy.

### EntourageGroupConfiguration
- Map \`group_type\` and \`render_as\` enums as strings (snake_case convention)
- \`display_order\` is required
- FK to \`wedding.profiles\`

### EntourageMemberConfiguration
- \`group_id\` NOT NULL, FK to \`wedding.entourage_groups\` with cascade delete
- \`display_order\` required

### EntourageCoupleConfiguration
- UNIQUE index on \`(group_id, member_a_id, member_b_id)\`
- CHECK constraint: \`member_a_id < member_b_id\`
- FK to both members with cascade delete

### Value converters needed
\`EntourageGroupIdConverter\`, \`EntourageMemberIdConverter\`, \`EntourageCoupleIdConverter\`" \
  "infrastructure"

create_issue \
  "[Infra] EF Core configs — StoryMoment, GalleryImage, GiftOption" \
  "$MS_WEDDINGS" \
  "## Scope

### StoryMomentConfiguration
- \`title\` and \`description\` NOT NULL
- \`display_order\` required

### GalleryImageConfiguration
- \`src_url\` and \`alt_text\` NOT NULL
- \`display_order\` required

### GiftOptionConfiguration
- Map \`gift_display_mode\` enum as string
- \`display_order\` required

### Value converters needed
\`StoryMomentIdConverter\`, \`GalleryImageIdConverter\`, \`GiftOptionIdConverter\`" \
  "infrastructure"

create_issue \
  "[Infra] EF Core configs — DressCodeConfig, DressCodeColor, DressCodeImage" \
  "$MS_WEDDINGS" \
  "## Scope

### DressCodeConfigConfiguration
- UNIQUE on \`wedding_id\` (1:1 with wedding profile)

### DressCodeColorConfiguration
- FK to \`wedding.dress_code_configs\` with cascade delete
- \`color_hex\` CHECK constraint: \`^#[0-9A-Fa-f]{3,8}$\`
- \`display_order\` required

### DressCodeImageConfiguration
- FK to \`wedding.dress_code_configs\` with cascade delete
- \`display_order\` required

### Value converters needed
\`DressCodeConfigIdConverter\`, \`DressCodeColorIdConverter\`, \`DressCodeImageIdConverter\`" \
  "infrastructure"

create_issue \
  "[Infra] EF Core configs — CeremonyAct, VendorCredit, Reminder, SnapShareConfig" \
  "$MS_WEDDINGS" \
  "## Scope

### CeremonyActConfiguration
- \`act_time\` maps to PostgreSQL \`time\` type (TimeOnly in .NET)
- \`display_order\` required

### VendorCreditConfiguration
- \`name\` and \`role\` NOT NULL

### ReminderConfiguration
- \`icon\`, \`title\`, \`body\` all NOT NULL

### SnapShareConfigConfiguration
- UNIQUE on \`wedding_id\` (1:1)

### Value converters needed
\`CeremonyActIdConverter\`, \`VendorCreditIdConverter\`, \`ReminderIdConverter\`
(SnapShareConfigId converter already exists in infrastructure)" \
  "infrastructure"

create_issue \
  "[Infra] Repository implementations — StoryMomentRepository, EntourageGroupRepository" \
  "$MS_WEDDINGS" \
  "## Scope

### StoryMomentRepository : IStoryMomentRepository
- \`GetAsync(StoryMomentId)\` → \`Task<StoryMoment?>\`
- \`GetByWeddingIdAsync(WeddingId)\` → ordered by \`display_order ASC\`
- \`Insert(StoryMoment)\`
- \`Update(StoryMoment)\`
- \`Delete(StoryMoment)\`

### EntourageGroupRepository : IEntourageGroupRepository
- \`GetAsync(EntourageGroupId)\` — includes Members + Couples
- \`GetByWeddingIdAsync(WeddingId)\` — includes Members + Couples, ordered by \`display_order ASC\`
- \`Insert\`, \`Update\`, \`Delete\`

Register both in \`WeddingsModule.cs\`." \
  "infrastructure"

create_issue \
  "[Infra] Repository implementations — GalleryImageRepository, GiftOptionRepository" \
  "$MS_WEDDINGS" \
  "## Scope

### GalleryImageRepository : IGalleryImageRepository
- \`GetAsync(GalleryImageId)\`
- \`GetByWeddingIdAsync(WeddingId)\` — ordered by display_order
- \`Insert\`, \`Update\`, \`Delete\`

### GiftOptionRepository : IGiftOptionRepository
- Same pattern as above

Register both in \`WeddingsModule.cs\`." \
  "infrastructure"

create_issue \
  "[Infra] Repository implementations — DressCodeConfigRepository, CeremonyActRepository" \
  "$MS_WEDDINGS" \
  "## Scope

### DressCodeConfigRepository : IDressCodeConfigRepository
- \`GetAsync(DressCodeConfigId)\` — includes Colors + Images
- \`GetByWeddingIdAsync(WeddingId)\` — includes Colors + Images, ordered by display_order
- \`Insert\`, \`Update\`

### CeremonyActRepository : ICeremonyActRepository
- \`GetAsync(CeremonyActId)\`
- \`GetByWeddingIdAsync(WeddingId)\` — ordered by display_order
- \`Insert\`, \`Update\`, \`Delete\`" \
  "infrastructure"

create_issue \
  "[Infra] Repository implementations — VendorCreditRepository, ReminderRepository" \
  "$MS_WEDDINGS" \
  "## Scope

### VendorCreditRepository : IVendorCreditRepository
### ReminderRepository : IReminderRepository

Standard pattern for both:
- \`GetAsync(Id)\`
- \`GetByWeddingIdAsync(WeddingId)\` — ordered by display_order
- \`Insert\`, \`Update\`, \`Delete\`

Register both in \`WeddingsModule.cs\` DI setup alongside existing \`WeddingRepository\`." \
  "infrastructure"

create_issue \
  "[Infra] Database migration — all remaining wedding aggregate tables" \
  "$MS_WEDDINGS" \
  "## Scope
Add an EF Core migration for all aggregate tables not covered by the initial \`InitialCreate\` migration.

### Tables to add
\`wedding.story_moments\`, \`wedding.entourage_groups\`, \`wedding.entourage_members\`, \`wedding.entourage_couples\`, \`wedding.gallery_images\`, \`wedding.gift_options\`, \`wedding.dress_code_configs\`, \`wedding.dress_code_colors\`, \`wedding.dress_code_images\`, \`wedding.ceremony_acts\`, \`wedding.vendor_credits\`, \`wedding.reminders\`, \`wedding.snap_share_configs\`

### Indexes to add (from database-design.md §4)
- \`idx_wedding_story_moments_wedding_id ON story_moments (wedding_id, display_order)\`
- \`idx_wedding_entourage_groups_wedding_id ON entourage_groups (wedding_id, display_order)\`
- \`idx_wedding_entourage_members_group_id ON entourage_members (group_id, display_order)\`
- \`idx_wedding_entourage_couples_group_id ON entourage_couples (group_id, display_order)\`
- \`idx_wedding_gallery_images_wedding_id ON gallery_images (wedding_id, display_order)\`

### Command
\`\`\`
dotnet ef migrations add AddWeddingAggregates -p Eternelle.Modules.Weddings.Infrastructure
\`\`\`" \
  "infrastructure"

create_issue \
  "[Infra] Domain event handlers (outbox) — all Weddings domain events" \
  "$MS_WEDDINGS" \
  "## Scope
Wire \`IdempotentDomainEventHandler<T>\` for every domain event raised in the Weddings module. Each handler is registered via the outbox pipeline and runs after \`SaveChanges\`.

### Events to handle
- \`WeddingCreatedDomainEvent\` → publish \`WeddingCreatedIntegrationEvent\` (consumed by Tenancy to create wedding profile row)
- \`WeddingDetailsUpdatedDomainEvent\`
- \`WeddingSnapShareUpdatedDomainEvent\`
- \`CeremonyActCreatedDomainEvent\`
- \`DressCodeConfigCreatedDomainEvent\`
- \`EntourageGroupCreatedDomainEvent\`
- \`GalleryImageCreatedDomainEvent\`
- \`GiftOptionCreatedDomainEvent\`
- \`ReminderCreatedDomainEvent\`
- \`StoryMomentCreatedDomainEvent\`
- \`VendorCreditCreatedDomainEvent\`

### Priority
\`WeddingCreatedDomainEvent\` → \`WeddingCreatedIntegrationEvent\` is the critical one. The rest can log or be no-ops initially." \
  "infrastructure" "cross-module"

# ── Presentation ──────────────────────────────────────────────────────────────

create_issue \
  "[API] Wedding profile + partner endpoints" \
  "$MS_WEDDINGS" \
  "## Scope
Minimal API endpoints for the Wedding aggregate (profile + partners).

### Endpoints
- \`GET    /weddings/{weddingId}\` — GetWedding
- \`PATCH  /weddings/{weddingId}/details\` — UpdateWeddingDetails
- \`PATCH  /weddings/{weddingId}/snap-share\` — UpdateSnapShareConfig
- \`POST   /weddings/{weddingId}/partners\` — AddPartner
- \`PUT    /weddings/{weddingId}/partners/{partnerId}\` — UpdatePartner

### Pattern
One \`IEndpoint\` class per action. Use \`Result.Match(Results.Ok, ApiResults.Problem)\`. Swagger tag: \`Weddings\`.
Auth: all endpoints require \`wedding:edit\` permission scoped to this wedding's tenant." \
  "api"

create_issue \
  "[API] StoryMoment endpoints — CRUD + reorder" \
  "$MS_WEDDINGS" \
  "## Endpoints
- \`GET    /weddings/{weddingId}/story-moments\`
- \`POST   /weddings/{weddingId}/story-moments\`
- \`PUT    /weddings/{weddingId}/story-moments/{id}\`
- \`DELETE /weddings/{weddingId}/story-moments/{id}\`
- \`PATCH  /weddings/{weddingId}/story-moments/reorder\` — body: \`{ ids: uuid[] }\`

Tag: \`StoryMoments\`" \
  "api"

create_issue \
  "[API] EntourageGroup + EntourageMember + EntourageCouple endpoints" \
  "$MS_WEDDINGS" \
  "## Endpoints

### Groups
- \`GET    /weddings/{weddingId}/entourage\` — full tree (groups + members + couples)
- \`POST   /weddings/{weddingId}/entourage/groups\`
- \`PUT    /weddings/{weddingId}/entourage/groups/{groupId}\`
- \`DELETE /weddings/{weddingId}/entourage/groups/{groupId}\`
- \`PATCH  /weddings/{weddingId}/entourage/groups/reorder\`

### Members
- \`POST   /entourage/groups/{groupId}/members\`
- \`PUT    /entourage/groups/{groupId}/members/{memberId}\`
- \`DELETE /entourage/groups/{groupId}/members/{memberId}\`
- \`PATCH  /entourage/groups/{groupId}/members/reorder\`

### Couples
- \`POST   /entourage/groups/{groupId}/couples\`
- \`DELETE /entourage/groups/{groupId}/couples/{coupleId}\`

Tag: \`Entourage\`" \
  "api"

create_issue \
  "[API] GalleryImage endpoints — CRUD + reorder" \
  "$MS_WEDDINGS" \
  "## Endpoints
- \`GET    /weddings/{weddingId}/gallery\`
- \`POST   /weddings/{weddingId}/gallery\`
- \`PUT    /weddings/{weddingId}/gallery/{id}\`
- \`DELETE /weddings/{weddingId}/gallery/{id}\`
- \`PATCH  /weddings/{weddingId}/gallery/reorder\`

Tag: \`Gallery\`" \
  "api"

create_issue \
  "[API] GiftOption endpoints — CRUD + reorder" \
  "$MS_WEDDINGS" \
  "## Endpoints
- \`GET    /weddings/{weddingId}/gift-options\`
- \`POST   /weddings/{weddingId}/gift-options\`
- \`PUT    /weddings/{weddingId}/gift-options/{id}\`
- \`DELETE /weddings/{weddingId}/gift-options/{id}\`
- \`PATCH  /weddings/{weddingId}/gift-options/reorder\`

Tag: \`GiftOptions\`" \
  "api"

create_issue \
  "[API] DressCodeConfig endpoints — config + colors + images" \
  "$MS_WEDDINGS" \
  "## Endpoints

### Config
- \`GET    /weddings/{weddingId}/dress-code\`
- \`POST   /weddings/{weddingId}/dress-code\`
- \`PUT    /weddings/{weddingId}/dress-code/{id}\`

### Colors
- \`POST   /dress-code/{id}/colors\`
- \`DELETE /dress-code/{id}/colors/{colorId}\`
- \`PATCH  /dress-code/{id}/colors/reorder\`

### Images
- \`POST   /dress-code/{id}/images\`
- \`DELETE /dress-code/{id}/images/{imageId}\`
- \`PATCH  /dress-code/{id}/images/reorder\`

Tag: \`DressCode\`" \
  "api"

create_issue \
  "[API] CeremonyAct endpoints — CRUD + reorder" \
  "$MS_WEDDINGS" \
  "## Endpoints
- \`GET    /weddings/{weddingId}/ceremony-acts\`
- \`POST   /weddings/{weddingId}/ceremony-acts\`
- \`PUT    /weddings/{weddingId}/ceremony-acts/{id}\`
- \`DELETE /weddings/{weddingId}/ceremony-acts/{id}\`
- \`PATCH  /weddings/{weddingId}/ceremony-acts/reorder\`

Tag: \`CeremonyActs\`" \
  "api"

create_issue \
  "[API] VendorCredit endpoints — CRUD + reorder" \
  "$MS_WEDDINGS" \
  "## Endpoints
- \`GET    /weddings/{weddingId}/vendor-credits\`
- \`POST   /weddings/{weddingId}/vendor-credits\`
- \`PUT    /weddings/{weddingId}/vendor-credits/{id}\`
- \`DELETE /weddings/{weddingId}/vendor-credits/{id}\`
- \`PATCH  /weddings/{weddingId}/vendor-credits/reorder\`

Tag: \`VendorCredits\`" \
  "api"

create_issue \
  "[API] Reminder endpoints — CRUD + reorder" \
  "$MS_WEDDINGS" \
  "## Endpoints
- \`GET    /weddings/{weddingId}/reminders\`
- \`POST   /weddings/{weddingId}/reminders\`
- \`PUT    /weddings/{weddingId}/reminders/{id}\`
- \`DELETE /weddings/{weddingId}/reminders/{id}\`
- \`PATCH  /weddings/{weddingId}/reminders/reorder\`

### Note
First-class tier only. Consider adding a tier check in the command handler rather than in the endpoint layer — keep policy in the application layer.

Tag: \`Reminders\`" \
  "api"

# ── Testing ───────────────────────────────────────────────────────────────────

create_issue \
  "[Test] Architecture tests — Weddings module layer boundaries" \
  "$MS_WEDDINGS" \
  "## Scope
NetArchTest.Rules assertions to enforce DDD layer boundaries.

### Rules to assert
- Domain has no dependency on Application, Infrastructure, or Presentation
- Application has no dependency on Infrastructure or Presentation
- Infrastructure references only Application and Domain
- All entities inherit from \`Eternelle.Common.Domain.Entity\`
- Repository interfaces are declared only in Domain
- Repository implementations are declared only in Infrastructure
- \`IEndpoint\` implementations are declared only in Presentation

### Project to create
\`Eternelle.Modules.Weddings.ArchitectureTests\`" \
  "testing"

create_issue \
  "[Test] Integration tests — Wedding profile + partner aggregate" \
  "$MS_WEDDINGS" \
  "## Scope
Full-stack integration tests using TestContainers (PostgreSQL) for the Wedding aggregate.

### Test cases
- CreateWedding → persisted; \`WeddingCreatedDomainEvent\` written to outbox
- UpdateWeddingDetails → fields updated; \`WeddingDetailsUpdatedDomainEvent\` in outbox
- AddPartner → stored with correct \`partner_number\`
- AddPartner twice with same partner_number → second call rejected
- UpdatePartner → fields updated
- GetWedding → returns correct response including partners

### Pattern
Follow Evently integration test pattern: spin up TestContainers PostgreSQL, run EF migrations, test through MediatR sender." \
  "testing"

create_issue \
  "[Test] Integration tests — StoryMoment, CeremonyAct, Reminder" \
  "$MS_WEDDINGS" \
  "## Scope
Integration tests for the three simplest ordered aggregates (same test structure for each).

### Per aggregate
- Create → persisted with correct \`display_order\`
- Update → fields mutated
- Delete → removed from DB
- Reorder → \`display_order\` updated correctly for all affected rows
- GetList → returns results ordered by \`display_order ASC\`" \
  "testing"

create_issue \
  "[Test] Integration tests — EntourageGroup, Member, Couple" \
  "$MS_WEDDINGS" \
  "## Scope

### Test cases
- CreateGroup → persisted with display_order
- AddMember to group → member stored with correct \`group_id\`
- PairCouple → stored; UUID ordering enforced (\`member_a_id < member_b_id\`)
- PairCouple duplicate → rejected with appropriate error
- RemoveMember → associated EntourageCouple rows also deleted
- GetEntourageGroups → returns full nested tree (groups → members → couples)" \
  "testing"

create_issue \
  "[Test] Integration tests — GalleryImage, GiftOption, DressCodeConfig, VendorCredit" \
  "$MS_WEDDINGS" \
  "## Scope

### GalleryImage
CRUD + reorder — standard pattern

### GiftOption
CRUD + reorder; validate \`DisplayMode = link\` requires \`LinkUrl\` not null

### DressCodeConfig
- Create config → persisted
- AddColor with invalid hex → rejected by \`HexColor\` value object
- AddColor + AddImage + Reorder both collections
- GetDressCodeConfig → returns config with ordered colors and images

### VendorCredit
CRUD + reorder — standard pattern" \
  "testing"

# =============================================================================
echo ""
echo "━━━ ISSUES — RSVP Module ━━━"

create_issue \
  "[Domain] Guest aggregate — GuestId, Guest, GuestErrors, IGuestRepository" \
  "$MS_RSVP" \
  "## Scope
Domain layer for the \`Guest\` aggregate root in the RSVP module.

### Files
- \`GuestId.cs\` — \`readonly record struct GuestId(Guid Value)\` with \`New()\` and \`Empty\`
- \`Guest.cs\` — \`Create(WeddingId, FirstName, LastName, Email?, MobileNumber?, PlusOneAllowed, TableNumber?, GroupName?)\`; generates \`LookupCode\` (crypto-random short token, e.g. \`K4X9-MJ2R\`)
- \`GuestErrors.cs\` — NotFound, DuplicateLookupCode
- \`IGuestRepository.cs\` — GetAsync(GuestId), GetByLookupCodeAsync(string), GetByWeddingIdAsync(WeddingId), Insert, Update, Delete
- \`GuestAddedDomainEvent.cs\`

### DB (rsvp.guests)
\`first_name text NOT NULL, last_name text NOT NULL, email text, mobile_number text, lookup_code text UNIQUE NOT NULL, plus_one_allowed boolean, table_number int, group_name text\`

### Note
\`lookup_code\` is generated by the domain — never accepted from the client." \
  "domain"

create_issue \
  "[Domain] RSVPSubmission aggregate — SubmissionId, Submission, ISubmissionRepository" \
  "$MS_RSVP" \
  "## Scope
Immutable append-only audit log of RSVP responses.

### Files
- \`RSVPSubmissionId.cs\`
- \`RSVPSubmission.cs\` — \`Submit(WeddingId, GuestId?, Name, Email?, MobileNumber?, Attending, GuestCount, PlusOneName?, PlusOneAttending?, DietaryRestrictions?, MealChoice?, Message?, IpAddress?)\`; generates \`ConfirmationId\`
- \`RSVPSubmissionErrors.cs\`
- \`IRSVPSubmissionRepository.cs\` — GetAsync(Id), GetByWeddingIdAsync(WeddingId, attending?), Insert
- \`RSVPSubmittedDomainEvent.cs\`

### Key design
No \`Update\` method — submissions are immutable. \`name\`, \`email\`, \`mobile_number\` are intentionally denormalized (audit trail captures what the guest typed, not the corrected guest list entry)." \
  "domain"

create_issue \
  "[Domain] CheckIn aggregate — CheckInId, CheckIn, ICheckInRepository, GuestCheckedInDomainEvent" \
  "$MS_RSVP" \
  "## Scope
Day-of attendance tracking. One row per guest per event they physically attend.

### Files
- \`CheckInId.cs\`
- \`CheckIn.cs\` — \`Create(GuestId, WeddingId, EventId?, CheckedInBy?)\`
- \`CheckInErrors.cs\` — AlreadyCheckedIn
- \`ICheckInRepository.cs\` — GetAsync(Id), GetByGuestAndEventAsync(GuestId, EventId?), GetByWeddingIdAsync(WeddingId, EventId?), Insert
- \`GuestCheckedInDomainEvent.cs\`

### DB constraint
\`UNIQUE (guest_id, event_id)\` — prevents double check-in to same event. Handler should return the existing check-in gracefully rather than erroring on duplicate scan." \
  "domain"

create_issue \
  "[App] Guest commands — AddGuest, UpdateGuest, RemoveGuest, BulkImportGuests" \
  "$MS_RSVP" \
  "## Scope

- \`AddGuestCommand(WeddingId, FirstName, LastName, Email?, MobileNumber?, PlusOneAllowed, TableNumber?, GroupName?)\`
- \`UpdateGuestCommand(GuestId, FirstName, LastName, Email?, MobileNumber?, PlusOneAllowed, TableNumber?, GroupName?)\`
- \`RemoveGuestCommand(GuestId)\`
- \`BulkImportGuestsCommand(WeddingId, IReadOnlyList<GuestImportRow>)\` — idempotent import; skips rows where lookup_code already exists

### Note
\`lookup_code\` is generated by \`Guest.Create()\` in the domain — never comes from the client or import file." \
  "application"

create_issue \
  "[App] Guest queries — GetGuests, GetGuestByLookupCode, GenerateGuestQRCode" \
  "$MS_RSVP" \
  "## Scope

- \`GetGuestsQuery(WeddingId, GroupName?)\` → paginated \`GuestResponse\` list; optionally include RSVP status per guest
- \`GetGuestByLookupCodeQuery(LookupCode)\` → \`GuestResponse\` — used by the public RSVP form to pre-fill fields
- \`GenerateGuestQRCodeQuery(GuestId)\` → returns the RSVP URL \`https://{hostname}/rsvp/{lookupCode}\`; QR image generation is client-side from this URL (no image stored in DB)" \
  "application"

create_issue \
  "[App] RSVP submission command + queries — SubmitRSVP, GetSubmissions, GetRSVPStats" \
  "$MS_RSVP" \
  "## Scope

### Command (public-facing, no auth)
- \`SubmitRSVPCommand(LookupCode, Name, Email?, MobileNumber?, Attending, GuestCount, PlusOneName?, PlusOneAttending?, DietaryRestrictions?, MealChoice?, Message?, IpAddress?)\`
- Resolve guest by lookup_code; link submission to guest_id; return \`confirmationId\`

### Queries (couple-facing, auth required)
- \`GetSubmissionsQuery(WeddingId, Attending?)\` → paginated list with guest details
- \`GetRSVPStatsQuery(WeddingId)\` → \`{ attending, notAttending, pending, plusOnes }\` — derived from submissions vs guest list" \
  "application"

create_issue \
  "[App] Check-in commands + queries — CheckInGuest, GetCheckIns, GetEventHeadCounts" \
  "$MS_RSVP" \
  "## Scope

### Command (coordinator, day-of)
- \`CheckInGuestCommand(LookupCode, EventId?, CheckedInBy?)\`
  - Resolve guest by lookup_code
  - If already checked in for this event: return existing check-in (do not 422)
  - Otherwise create CheckIn row

### Queries
- \`GetCheckInsQuery(WeddingId, EventId?)\` → list of checked-in guests for an event
- \`GetEventHeadCountsQuery(WeddingId)\` → \`IReadOnlyList<{ EventId, Count }>\` + total" \
  "application"

create_issue \
  "[Infra] RSVPDbContext, EF Core configs, repositories, migration" \
  "$MS_RSVP" \
  "## Scope

### DbContext
\`RSVPDbContext\` implementing \`IUnitOfWork\`, schema: \`rsvp\`. Include Outbox + Inbox message tables.

### Entity Configs
- \`GuestConfiguration\` — \`lookup_code\` UNIQUE NOT NULL; names NOT NULL
- \`RSVPSubmissionConfiguration\` — \`confirmation_id\` UNIQUE; no \`Update\` method (immutable)
- \`CheckInConfiguration\` — UNIQUE index on \`(guest_id, event_id)\`

### Repositories
\`GuestRepository\`, \`RSVPSubmissionRepository\`, \`CheckInRepository\`

### Indexes
- \`idx_rsvp_guests_lookup_code ON rsvp.guests (lookup_code)\` — QR scan hot path
- \`idx_rsvp_guests_wedding_id ON rsvp.guests (wedding_id)\`
- \`idx_rsvp_submissions_wedding_id ON rsvp.submissions (wedding_id)\`
- \`idx_rsvp_check_ins_wedding_event ON rsvp.check_ins (wedding_id, event_id)\`

### Value converters
\`GuestIdConverter\`, \`RSVPSubmissionIdConverter\`, \`CheckInIdConverter\`" \
  "infrastructure"

create_issue \
  "[Infra] RSVPModule.cs — DI setup, outbox handlers, integration event consumers" \
  "$MS_RSVP" \
  "## Scope
- \`RSVPModule.cs\` — DI registration (DbContext, UoW, repositories, endpoints, outbox/inbox jobs)
- \`RSVPSubmittedDomainEvent\` handler → optionally publish \`RSVPSubmittedIntegrationEvent\`
- Consume \`WeddingCreatedIntegrationEvent\` from Weddings module if RSVP needs to react on wedding creation" \
  "infrastructure" "cross-module"

create_issue \
  "[API] Guest management endpoints — couple-facing (auth required)" \
  "$MS_RSVP" \
  "## Endpoints (require rsvp:manage permission)
- \`GET    /rsvp/{weddingId}/guests\` — list with optional group_name filter
- \`POST   /rsvp/{weddingId}/guests\` — add guest
- \`PUT    /rsvp/{weddingId}/guests/{guestId}\` — update guest
- \`DELETE /rsvp/{weddingId}/guests/{guestId}\` — remove guest
- \`POST   /rsvp/{weddingId}/guests/import\` — bulk import (multipart or JSON array)
- \`GET    /rsvp/{weddingId}/guests/{guestId}/qr-url\` — returns RSVP URL for QR generation

Tag: \`RSVP.Guests\`" \
  "api"

create_issue \
  "[API] RSVP submission endpoint — public-facing (no auth)" \
  "$MS_RSVP" \
  "## Endpoints

### Public (no auth, rate-limited by IP)
- \`GET  /rsvp/{lookupCode}\` — returns guest pre-fill data for the form
- \`POST /rsvp/{lookupCode}/submit\` — submit RSVP; returns \`{ confirmationId }\`

### Couple dashboard (rsvp:manage permission)
- \`GET /rsvp/{weddingId}/submissions\` — paginated, filterable by attending status
- \`GET /rsvp/{weddingId}/stats\` — attending / not attending / pending / plus-one counts

Tag: \`RSVP.Submissions\`" \
  "api"

create_issue \
  "[API] Check-in endpoints — coordinator-facing" \
  "$MS_RSVP" \
  "## Endpoints
- \`POST /rsvp/{lookupCode}/check-in\` — body: \`{ eventId?: uuid, checkedInBy?: string }\`; idempotent
- \`GET  /rsvp/{weddingId}/check-ins\` — filter by eventId
- \`GET  /rsvp/{weddingId}/head-counts\` — per-event check-in counts

Tag: \`RSVP.CheckIn\`" \
  "api"

create_issue \
  "[Test] Architecture + integration tests — RSVP module" \
  "$MS_RSVP" \
  "## Architecture tests
NetArchTest layer boundaries for RSVP module.

## Integration tests

### Guest management
- AddGuest → lookup_code generated; persisted
- BulkImportGuests → idempotent; existing lookup_codes skipped

### Submission flow
- SubmitRSVP via lookup_code → submission created; confirmation_id returned
- Second SubmitRSVP for same guest → new submission row (append-only, not rejected)
- SubmitRSVP with invalid lookup_code → 404

### Check-in flow
- CheckInGuest → check-in created
- Duplicate scan (same guest + event) → existing check-in returned, no error
- GetEventHeadCounts → correct per-event counts after check-ins

### Stats
- GetRSVPStats → correct attending/not-attending/pending after mixed submissions" \
  "testing"

# =============================================================================
echo ""
echo "━━━ ISSUES — Guestbook Module ━━━"

create_issue \
  "[Domain] GuestbookEntry aggregate — EntryId, Entry, IGuestbookEntryRepository" \
  "$MS_GUESTBOOK" \
  "## Scope
- \`GuestbookEntryId.cs\`
- \`GuestbookEntry.cs\` — \`Submit(WeddingId, GuestName, Message, IpAddress?)\`; \`Approve()\`; \`Reject()\`; \`is_approved\` defaults to \`true\`
- \`GuestbookEntryErrors.cs\` — NotFound
- \`IGuestbookEntryRepository.cs\` — GetAsync(EntryId), GetByWeddingIdAsync(WeddingId, approvedOnly), Insert, Update, Delete
- \`GuestbookEntrySubmittedDomainEvent.cs\`

### DB (guestbook.entries)
\`guest_name text NOT NULL, message text NOT NULL, is_approved boolean DEFAULT true, ip_address inet, submitted_at timestamptz DEFAULT now()\`" \
  "domain"

create_issue \
  "[App] Guestbook commands — SubmitEntry, ApproveEntry, DeleteEntry" \
  "$MS_GUESTBOOK" \
  "## Scope

- \`SubmitGuestbookEntryCommand(WeddingId, GuestName, Message, IpAddress?)\` — public, no auth required
- \`ApproveGuestbookEntryCommand(EntryId)\` — requires \`guestbook:moderate\` permission
- \`DeleteGuestbookEntryCommand(EntryId)\` — requires \`guestbook:moderate\` permission" \
  "application"

create_issue \
  "[App] Guestbook query — GetGuestbookEntries" \
  "$MS_GUESTBOOK" \
  "## Scope
- \`GetGuestbookEntriesQuery(WeddingId, ApprovedOnly, Page, PageSize)\`
- Returns paginated \`GuestbookEntryResponse\`
- Ordered by \`submitted_at DESC\`
- Public callers: \`approvedOnly = true\`
- Couple dashboard: \`approvedOnly = false\` (shows pending for moderation)" \
  "application"

create_issue \
  "[Infra] GuestbookDbContext, configs, repository, migration" \
  "$MS_GUESTBOOK" \
  "## Scope
- \`GuestbookDbContext\` implementing \`IUnitOfWork\`, schema: \`guestbook\`
- \`GuestbookEntryConfiguration\` — \`guest_name\` and \`message\` NOT NULL; \`is_approved\` defaults to \`true\`; \`ip_address\` maps to \`inet\`
- \`GuestbookEntryRepository : IGuestbookEntryRepository\`
- \`GuestbookEntryIdConverter\`
- Index: \`idx_guestbook_entries_wedding_id ON entries (wedding_id, submitted_at DESC)\`
- Migration: \`InitialCreate\`" \
  "infrastructure"

create_issue \
  "[Infra] GuestbookModule.cs — DI setup + domain event handlers" \
  "$MS_GUESTBOOK" \
  "## Scope
- \`GuestbookModule.cs\` — DI registration (DbContext, UoW, repository, endpoints, outbox/inbox)
- \`GuestbookEntrySubmittedDomainEvent\` handler — log or publish integration event as needed
- Consume \`WeddingCreatedIntegrationEvent\` from Weddings if guestbook needs to react on new wedding" \
  "infrastructure" "cross-module"

create_issue \
  "[API] Guestbook endpoints — public submission + couple moderation" \
  "$MS_GUESTBOOK" \
  "## Endpoints

### Public (no auth)
- \`POST /guestbook/{weddingId}/entries\` — submit a message
- \`GET  /guestbook/{weddingId}\` — approved entries only (paginated)

### Couple dashboard (guestbook:moderate permission)
- \`GET    /guestbook/{weddingId}/entries\` — all entries including pending
- \`PATCH  /guestbook/{weddingId}/entries/{id}/approve\`
- \`DELETE /guestbook/{weddingId}/entries/{id}\`

Tag: \`Guestbook\`" \
  "api"

create_issue \
  "[Test] Architecture + integration tests — Guestbook module" \
  "$MS_GUESTBOOK" \
  "## Architecture tests
Layer boundary rules for Guestbook module.

## Integration tests
- SubmitEntry → stored with \`is_approved = true\` by default
- ApproveEntry → \`is_approved\` flipped to true (if moderation default changes to false later)
- DeleteEntry → removed from DB
- GetEntries (public) → returns only \`is_approved = true\` rows
- GetEntries (couple) → returns all rows including pending
- Pagination: ordered by \`submitted_at DESC\`" \
  "testing"

# =============================================================================
echo ""
echo "━━━ ISSUES — IAM Module ━━━"

create_issue \
  "[Domain] User aggregate — UserId, User, IUserRepository, UserProvisionedDomainEvent" \
  "$MS_IAM" \
  "## Scope
Shadow user profile — Keycloak owns credentials; IAM module owns app context.

### Files
- \`UserId.cs\`
- \`User.cs\` — \`Provision(KeycloakUserId, Email, DisplayName?)\`; \`SyncProfile(Email, DisplayName?)\`
- \`UserErrors.cs\` — NotFound, AlreadyProvisioned
- \`IUserRepository.cs\` — GetAsync(UserId), GetByKeycloakIdAsync(string), GetByEmailAsync(string), Insert, Update
- \`UserProvisionedDomainEvent.cs\`

### DB (iam.users)
\`keycloak_user_id text UNIQUE NOT NULL, email text UNIQUE NOT NULL, display_name text\`

### Key rule
\`keycloak_user_id\` is the only stable cross-system identifier. Never use email as a FK." \
  "domain"

create_issue \
  "[Domain] Role, Permission, UserPermissionGrant entities" \
  "$MS_IAM" \
  "## Scope
RBAC building blocks.

### Files
- \`RoleId.cs\`, \`Role.cs\` — name UNIQUE, description; named roles: \`couple\`, \`super_admin\`
- \`PermissionId.cs\`, \`Permission.cs\` — name UNIQUE (e.g. \`wedding:edit\`, \`rsvp:manage\`)
- \`UserPermissionGrantId.cs\`, \`UserPermissionGrant.cs\` — \`Grant(UserId, PermissionId, ResourceType?, ResourceId?)\`
- \`IRoleRepository.cs\`, \`IPermissionRepository.cs\`, \`IUserPermissionGrantRepository.cs\`

### Permission names (seed data)
\`tenant:edit\`, \`tenant:publish\`, \`wedding:edit\`, \`rsvp:manage\`, \`guestbook:moderate\`, \`catalog:manage\`" \
  "domain"

create_issue \
  "[App] User provisioning command — ProvisionUser (JIT on first JWT login)" \
  "$MS_IAM" \
  "## Scope
Called by JWT middleware on every request when \`keycloak_user_id\` is not yet in the DB.

### Command
- \`ProvisionUserCommand(KeycloakUserId, Email, DisplayName?)\`
- Handler: check \`IUserRepository.GetByKeycloakIdAsync()\` → if not found, create; if found, sync email/displayName
- Returns \`Result<UserId>\`
- Raises \`UserProvisionedDomainEvent\`

### Important
This command must be idempotent — every request from a new Keycloak user triggers it." \
  "application"

create_issue \
  "[App] Permission resolution query — GetUserPermissions (IPermissionService)" \
  "$MS_IAM" \
  "## Scope
Core of authorization. Called on every authenticated request via \`CustomClaimsTransformation\`.

### Query
- \`GetUserPermissionsQuery(UserId, ResourceType?, ResourceId?)\` → \`Result<PermissionsResponse>\`

### Resolution order (from database-design.md §8)
1. Is \`super_admin\`? → grant all permissions immediately
2. Resource-scoped grant matching \`(permission, resource_type, resource_id)\`? → allow
3. Role-based permission with no resource scope? → allow
4. → deny

### Performance
Cache results per user via \`ICacheService\` (Redis) with short TTL (e.g. 5 min). Invalidate on role/permission changes." \
  "application"

create_issue \
  "[App] Role + permission grant commands — AssignRole, GrantPermission, GrantTenantPermissions" \
  "$MS_IAM" \
  "## Scope

- \`AssignRoleCommand(UserId, RoleId)\`
- \`RevokeRoleCommand(UserId, RoleId)\`
- \`GrantPermissionCommand(UserId, PermissionId, ResourceType?, ResourceId?)\`
- \`RevokePermissionCommand(UserId, PermissionId, ResourceType?, ResourceId?)\`
- \`GrantTenantPermissionsCommand(UserId, TenantId)\` — convenience command called during tenant creation to scope the couple's permissions to their new tenant (see database-design.md §8 seed hook)" \
  "application"

create_issue \
  "[Infra] IAMDbContext, EF Core configs, repositories, migration + seed data" \
  "$MS_IAM" \
  "## Scope

### DbContext
\`IAMDbContext\` schema: \`iam\`

### Entity Configs
- \`UserConfiguration\` — keycloak_user_id UNIQUE, email UNIQUE
- \`RoleConfiguration\`, \`PermissionConfiguration\`
- \`UserRoleConfiguration\` — composite PK (user_id, role_id)
- \`RolePermissionConfiguration\` — composite PK (role_id, permission_id)
- \`UserPermissionGrantConfiguration\` — UNIQUE (user_id, permission_id, resource_type, resource_id)

### Repositories
\`UserRepository\`, \`RoleRepository\`, \`PermissionRepository\`, \`UserPermissionGrantRepository\`

### Indexes
- \`idx_iam_users_keycloak_user_id ON iam.users (keycloak_user_id)\`
- \`idx_iam_user_roles_user_id ON iam.user_roles (user_id)\`
- \`idx_iam_user_permission_grants_user_id ON iam.user_permission_grants (user_id, resource_type, resource_id)\`

### Migration + Seed
Include roles (super_admin, couple) and all 6 permissions from database-design.md §8. Seed role_permissions mapping (couple gets all except catalog:manage)." \
  "infrastructure"

create_issue \
  "[Infra] IPermissionService implementation + JWT middleware wiring" \
  "$MS_IAM" \
  "## Scope
- \`PermissionService : IPermissionService\` — implements \`GetPermissionsAsync(userId)\` using the resolution order from GetUserPermissionsQuery
- \`CustomClaimsTransformation\` — on each authenticated request: extract JWT \`sub\` → JIT-provision user → load permissions → attach as claims
- Cache permission sets per UserId via \`ICacheService\` (Redis), invalidate on role/grant changes
- \`IAMModule.cs\` — full DI registration" \
  "infrastructure"

create_issue \
  "[API] IAM endpoints — user profile + admin RBAC management" \
  "$MS_IAM" \
  "## Endpoints

### Authenticated user
- \`GET /users/me\` — returns current user profile

### Admin only (super_admin)
- \`POST   /admin/users/{userId}/roles\` — assign role
- \`DELETE /admin/users/{userId}/roles/{roleId}\` — revoke role
- \`POST   /admin/users/{userId}/permissions\` — grant scoped permission
- \`DELETE /admin/users/{userId}/permissions/{permissionId}\` — revoke permission

Tag: \`IAM\`" \
  "api"

create_issue \
  "[Test] Architecture + integration tests — IAM module" \
  "$MS_IAM" \
  "## Architecture tests
Layer boundaries for IAM module.

## Integration tests
- ProvisionUser → created on first call; synced on subsequent (idempotent)
- GetUserPermissions → super_admin returns all; couple returns scoped permissions only
- AssignRole → user gains role's permissions
- GrantPermission (resource-scoped) → resolves correctly for that resource, not others
- PermissionService caching → second call returns cached result; invalidation works" \
  "testing"

# =============================================================================
echo ""
echo "━━━ ISSUES — Tenancy Module ━━━"

create_issue \
  "[Domain] Tenant aggregate — TenantId, Tenant, ITenantRepository, domain events" \
  "$MS_TENANCY" \
  "## Scope
- \`TenantId.cs\`
- \`Tenant.cs\` — \`Create(OwnerUserId, Slug, Tier)\`; \`Publish()\`; \`Unpublish()\`; \`AssignTemplate(TemplateId, ThemeIndex)\`; \`AddHostname(Hostname, IsPrimary)\`; \`UpdateSectionEntries(IReadOnlyList<SectionEntry>)\`
- \`TenantErrors.cs\` — NotFound, SlugTaken, AlreadyPublished, TemplateTierMismatch
- \`ITenantRepository.cs\` — GetAsync(TenantId), GetByHostnameAsync(string), GetBySlugAsync(string), Insert, Update
- \`TenantCreatedDomainEvent.cs\`, \`TenantPublishedDomainEvent.cs\`

### Child entities
- \`TenantHostname.cs\` (TenantHostnameId, Hostname, IsPrimary)
- \`TenantTemplateConfig.cs\` (TemplateId as string, ThemeIndex)
- \`TenantSectionEntry.cs\` (SectionId enum string, Enabled, DisplayOrder, Variant, OverrideTitle, OverrideSubtitle)" \
  "domain"

create_issue \
  "[App] Tenant commands — CreateTenant, PublishTenant, UpdateTemplate, UpdateSectionManifest" \
  "$MS_TENANCY" \
  "## Scope

- \`CreateTenantCommand(OwnerUserId, Slug, Tier)\` — also calls \`GrantTenantPermissionsCommand\` in IAM module (cross-module)
- \`PublishTenantCommand(TenantId)\` / \`UnpublishTenantCommand(TenantId)\`
- \`UpdateTenantTemplateCommand(TenantId, TemplateId, ThemeIndex)\` — validates template exists and tier compatibility
- \`UpdateSectionManifestCommand(TenantId, IReadOnlyList<SectionEntryInput>)\` — upserts section entries with display_order and variant
- \`AddTenantHostnameCommand(TenantId, Hostname, IsPrimary)\`" \
  "application" "cross-module"

create_issue \
  "[App] Tenant queries — GetTenantByHostname, GetTenantById, GetSectionManifest" \
  "$MS_TENANCY" \
  "## Scope

- \`GetTenantByHostnameQuery(Hostname)\` → \`TenantResponse\` — hot path; every public request; result is cacheable
- \`GetTenantByIdQuery(TenantId)\` → \`TenantResponse\` with section entries + template config
- \`GetSectionManifestQuery(TenantId)\` → ordered list of \`SectionEntryResponse\` with variant, enabled, overrides" \
  "application"

create_issue \
  "[Infra] TenancyDbContext, EF Core configs, repositories, migration" \
  "$MS_TENANCY" \
  "## Scope

### DbContext
\`TenancyDbContext\` schema: \`tenancy\`

### Entity Configs
- \`TenantConfiguration\` — \`slug\` UNIQUE; \`tier\` as \`tenancy.tenant_tier\` enum
- \`TenantHostnameConfiguration\` — \`hostname\` UNIQUE; FK to tenant with cascade
- \`TenantTemplateConfigConfiguration\` — \`tenant_id\` UNIQUE (1:1); \`template_id\` is text FK
- \`TenantSectionEntryConfiguration\` — UNIQUE (tenant_id, section_id); \`section_id\` stored as string matching catalog enum values

### Repositories
\`TenantRepository\` — \`GetByHostnameAsync\` joins \`tenant_hostnames\` using the index hot path

### Indexes
- \`idx_tenant_hostnames_hostname ON tenancy.tenant_hostnames (hostname)\`
- \`idx_tenants_slug ON tenancy.tenants (slug)\`" \
  "infrastructure"

create_issue \
  "[Infra] TenancyModule.cs + integration event consumer (TenantCreated → Wedding)" \
  "$MS_TENANCY" \
  "## Scope
- \`TenancyModule.cs\` — full DI registration
- \`TenantCreatedDomainEvent\` handler → publish \`TenantCreatedIntegrationEvent\`
- Weddings module consumes \`TenantCreatedIntegrationEvent\` to create the \`wedding.profiles\` row for the new tenant" \
  "infrastructure" "cross-module"

create_issue \
  "[API] Tenancy endpoints — tenant management + hostname resolution" \
  "$MS_TENANCY" \
  "## Endpoints

### Couple-facing (auth required)
- \`POST  /tenants\` — create tenant
- \`GET   /tenants/{tenantId}\` — get details
- \`PATCH /tenants/{tenantId}/publish\`
- \`PUT   /tenants/{tenantId}/template\` — update template + theme
- \`PUT   /tenants/{tenantId}/sections\` — update section manifest
- \`POST  /tenants/{tenantId}/hostnames\` — add hostname

### Internal (no public auth)
- \`GET /internal/resolve-tenant?hostname={hostname}\` — used by frontend middleware

Tag: \`Tenancy\`" \
  "api"

create_issue \
  "[Test] Architecture + integration tests — Tenancy module" \
  "$MS_TENANCY" \
  "## Architecture tests
Layer boundaries for Tenancy module.

## Integration tests
- CreateTenant → slug unique constraint enforced
- PublishTenant → \`published\` flipped; cannot publish twice
- UpdateTemplate → tier mismatch rejected; valid assignment persisted
- UpdateSectionManifest → all entries upserted with correct display_order
- GetTenantByHostname → resolves via hostname join; returns primary hostname" \
  "testing"

# =============================================================================
echo ""
echo "━━━ ISSUES — Catalog Module ━━━"

create_issue \
  "[Domain] Template aggregate — TemplateId, Template, ITemplateRepository" \
  "$MS_CATALOG" \
  "## Scope

### Files
- \`TemplateId.cs\` — text-based ID (e.g. 'kapilya', 'diwata', 'sage'), not uuid
- \`Template.cs\` — \`Create(Id, Name, Description?, Tier, PreviewImageUrl?, Features?)\`; \`AddTheme(ThemeIndex, Name, Colors, Typography, Textures?, IsDark)\`; \`AddSupportedSection(SectionId, DisplayOrder)\`; \`AddSectionVariant(SectionId, Variant, IsDefault)\`
- \`TemplateErrors.cs\` — NotFound, DuplicateThemeIndex, SectionAlreadySupported, DuplicateDefaultVariant
- \`ITemplateRepository.cs\` — GetAsync(TemplateId), GetAllAsync(tier?), Insert, Update

### Child entities
- \`TemplateTheme.cs\` — ThemeIndex, Name, Colors (jsonb), Typography (jsonb), Textures (jsonb?), IsDark
- \`TemplateSupportedSection.cs\` — SectionId (enum string), DisplayOrder
- \`TemplateSectionVariant.cs\` — SectionId (enum string), Variant string, IsDefault bool" \
  "domain"

create_issue \
  "[App] Catalog commands — CreateTemplate, AddTheme, AddSupportedSection, AddSectionVariant" \
  "$MS_CATALOG" \
  "## Scope
Admin-only commands (require \`catalog:manage\` permission).

- \`CreateTemplateCommand(Id, Name, Description?, Tier, PreviewImageUrl?, Features?)\`
- \`UpdateTemplateCommand(TemplateId, Name, Description?, PreviewImageUrl?, Features?)\`
- \`AddTemplateThemeCommand(TemplateId, ThemeIndex, Name, Colors, Typography, Textures?, IsDark)\`
- \`AddSupportedSectionCommand(TemplateId, SectionId, DisplayOrder)\`
- \`AddSectionVariantCommand(TemplateId, SectionId, Variant, IsDefault)\`" \
  "application"

create_issue \
  "[App] Catalog queries — GetTemplates, GetTemplate, GetSupportedSections, GetSectionVariants" \
  "$MS_CATALOG" \
  "## Scope

- \`GetTemplatesQuery(Tier?)\` → \`IReadOnlyList<TemplateSummaryResponse>\` (public)
- \`GetTemplateQuery(TemplateId)\` → full template with themes, sections, variants
- \`GetSupportedSectionsQuery(TemplateId)\` → ordered sections for a template
- \`GetSectionVariantsQuery(TemplateId, SectionId)\` → available variants for the section picker UI in the editor" \
  "application"

create_issue \
  "[Infra] CatalogDbContext, EF Core configs, repository, migration + seed data" \
  "$MS_CATALOG" \
  "## Scope

### DbContext
\`CatalogDbContext\` schema: \`catalog\`

### Entity Configs
- \`TemplateConfiguration\` — \`id\` is \`text\` primary key (not uuid); no \`created_at\` auto-value from EF
- \`TemplateThemeConfiguration\` — UNIQUE (template_id, theme_index); colors/typography/textures as jsonb columns
- \`TemplateSupportedSectionConfiguration\` — UNIQUE (template_id, section_id)
- \`TemplateSectionVariantConfiguration\` — UNIQUE (template_id, section_id, variant); partial unique index for \`is_default = true\` per section

### Migration + Seed
Seed at least one template (e.g. 'kapilya') with 2+ themes and the core supported sections to unblock Tenancy module development. See database-design.md §3.3 for enum values." \
  "infrastructure"

create_issue \
  "[Infra] CatalogModule.cs — DI setup + caching strategy" \
  "$MS_CATALOG" \
  "## Scope
- \`CatalogModule.cs\` — DI registration (DbContext, UoW, repository, endpoints)
- No Outbox/Inbox needed initially — catalog data changes only via admin commands
- Cache template reads via \`ICacheService\` (Redis) since templates change rarely; invalidate on update commands" \
  "infrastructure"

create_issue \
  "[API] Catalog endpoints — public template listing + admin management" \
  "$MS_CATALOG" \
  "## Endpoints

### Public (no auth)
- \`GET /catalog/templates\` — list; filter by tier
- \`GET /catalog/templates/{id}\` — full details with themes and sections
- \`GET /catalog/templates/{id}/sections/{sectionId}/variants\` — variant picker data

### Admin (catalog:manage permission)
- \`POST /catalog/templates\`
- \`PUT  /catalog/templates/{id}\`
- \`POST /catalog/templates/{id}/themes\`
- \`POST /catalog/templates/{id}/sections\`
- \`POST /catalog/templates/{id}/sections/{sectionId}/variants\`

Tag: \`Catalog\`" \
  "api"

create_issue \
  "[Test] Architecture + integration tests — Catalog module" \
  "$MS_CATALOG" \
  "## Architecture tests
Layer boundaries for Catalog module.

## Integration tests
- CreateTemplate → persisted with text PK
- AddTheme → UNIQUE (template_id, theme_index) enforced
- AddSectionVariant → only one \`is_default = true\` per section per template
- GetTemplates (filter by tier) → correct results
- GetSupportedSections → ordered by \`display_order\`
- GetSectionVariants → returns all variants with correct \`is_default\` flag" \
  "testing"

# =============================================================================
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅  Done!"
echo ""
echo "    View issues:     https://github.com/${REPO}/issues"
echo "    View milestones: https://github.com/${REPO}/milestones"
echo ""
echo "💡  Tip: Set milestone due dates from the GitHub UI."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
