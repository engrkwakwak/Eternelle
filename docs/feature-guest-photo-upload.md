# Feature: Guest Photo Upload (SnapShare)

**Status: In Progress — presign flow fully implemented with MinIO (local) / S3-compatible CDN (production), Redis slot store, and enum-based subscription tiers.**

Guests scan a QR code at the wedding and upload photos directly from their phones. Photos are moderated (auto or manual) before appearing on the public feed on the couple's wedding website.

---

## Overview

The upload flow is split into two steps to support bulk uploads efficiently. Instead of routing file bytes through the API server, guests upload directly to the CDN. The server only issues presigned URLs and stores the resulting CDN URLs as metadata.

```text
Guest scans QR code
       │
       ▼
GET /photos/upload?token=         ← fetch upload page context (weddingId, uploaderNameRequired)
       │
       ▼
POST /photos/upload/presign?token=  ← request N upload slots from server
       │  returns: [{ slotId, presignedUrl, cdnUrl }]
       │
       ▼
Upload N files in parallel to CDN   ← browser PUTs each file to its presignedUrl
       │
       ▼
POST /photos/upload?token=          ← register all photos in one batch
       │  body: [{ slotId, uploaderName?, widthPx?, heightPx? }]
       │  returns: { photoIds: [...] }
       │
       ▼
Photos stored (pending or approved depending on moderation mode)
```

---

## Why Presigned URLs?

The alternative — POSTing file bytes to the API — routes all upload bandwidth through the server. For a wedding where guests might upload 100+ photos in bulk, this creates memory pressure and slows response times.

With presigned URLs:
- The server never touches the file bytes
- All N uploads happen in parallel in the browser
- The server stays lightweight regardless of file size or count
- The CDN URL is known at presign time, so the server controls what gets stored (no arbitrary URLs)

---

## Endpoints

All upload endpoints are **public** (no JWT required). Access is controlled by the `uploadToken` — an opaque token embedded in the QR code.

### GET /photos/upload?token={uploadToken}

Fetch context needed to render the upload UI.

**Response `200 OK`**

```json
{
  "weddingId": "018f1a2b-...",
  "uploaderNameRequired": false
}
```

`uploaderNameRequired` is driven by `SnapShareConfig.UploaderNameRequired` on the wedding's snap-share configuration. The couple sets this via `PUT /weddings/{weddingId}/snap-share`.

**Response `404 Not Found`** — token is invalid or expired. The same error is returned whether the token never existed or the wedding was deleted. This is intentional — do not distinguish the two cases.

---

### POST /photos/upload/presign?token={uploadToken}

Step 1. Request presigned upload slots from the server.

**Request body**

```json
{
  "count": 5
}
```

| Field   | Type  | Rules                                    |
|---------|-------|------------------------------------------|
| `count` | `int` | Required. Min `1`, max `30` per request. |

**Response `200 OK`**

```json
{
  "uploads": [
    {
      "slotId": "018f1a2b-...",
      "presignedUrl": "https://cdn.eternelle.ph/presign/...",
      "cdnUrl": "https://cdn.eternelle.ph/images/..."
    }
  ]
}
```

| Field          | Description                                                                 |
|----------------|-----------------------------------------------------------------------------|
| `slotId`       | Server-generated ID. Pass this back in Step 2 to redeem the slot.           |
| `presignedUrl` | CDN presigned URL. Upload the file here using a `PUT`.                          |
| `cdnUrl`       | The final URL of the uploaded image. Known at presign time.                  |

Slots expire after **15 minutes**. If the guest doesn't complete the upload within that window, the slot is invalidated and they must start over.

**Response `404 Not Found`** — invalid token (same generic error, no information leakage).

**Response `422 Unprocessable Entity`** — `count` is 0, negative, or exceeds 30.

**Response `422 Unprocessable Entity`** — plan photo limit would be exceeded by this batch. The couple's subscription plan caps the total number of **active (non-`OverLimit`) photos**, including those pending manual moderation — not just approved ones.

---

### POST /photos/upload?token={uploadToken}

Step 2. Register photos after uploading to the CDN.

**Rate limit:** 10 requests per minute per IP address. Returns `429 Too Many Requests` when exceeded.

**Request body**

```json
{
  "photos": [
    {
      "slotId": "018f1a2b-...",
      "uploaderName": "Juan dela Cruz",
      "widthPx": 3024,
      "heightPx": 4032
    }
  ]
}
```

| Field          | Type     | Rules                                                       |
|----------------|----------|-------------------------------------------------------------|
| `slotId`       | `guid`   | Required. Must be a slot issued by Step 1, not yet redeemed. |
| `uploaderName` | `string` | Optional. Max 100 characters.                                |
| `widthPx`      | `int`    | Optional. Must be > 0 if provided. Read from the file.       |
| `heightPx`     | `int`    | Optional. Must be > 0 if provided. Read from the file.       |

**Response `200 OK`**

```json
{
  "photoIds": [
    "018f1a2b-...",
    "018f1a2c-..."
  ]
}
```

**Response `422 Unprocessable Entity`** — any `slotId` is invalid, expired, or already redeemed. The **entire batch** is rejected — no partial registrations. The guest must presign and re-upload the failed photos.

**Response `422 Unprocessable Entity`** — plan limit exceeded.

---

## Moderation

Whether photos appear immediately or require couple approval depends on `SnapShareConfig.ModerationMode`:

| Mode     | Behaviour                                                                     |
|----------|-------------------------------------------------------------------------------|
| `Auto`   | Photos are stored as `approved` and appear in the public feed immediately.    |
| `Manual` | Photos are stored as `pending` and must be approved by the couple via their dashboard. |

If the plan limit is hit **after** the fast-path check (e.g. due to concurrent uploads), the enforcement query runs inside the same transaction and marks over-limit photos as `OverLimit`. These photos are hidden from both the public feed and the couple's dashboard.

---

## The Upload Token

The `uploadToken` is a `Guid` stored on `SnapShareConfig`. It is embedded in the QR code URL:

```text
https://app.eternelle.ph/photos/upload?token={uploadToken}
```

The couple can **regenerate** the token via their dashboard (`POST /weddings/{weddingId}/snap-share/regenerate-token`). Regenerating the token immediately invalidates all previously distributed QR codes. The old token becomes useless — no new slots can be generated and no new photos can be registered using it.

---

## Public Feed

Approved photos are served at:

```text
GET /weddings/{weddingId}/photos/feed
```

No authentication required. The response is cacheable for 30 seconds (`Cache-Control: public, max-age=30`). Only `status = approved` photos are included. `status` and `reviewedAt` are intentionally omitted from the public response.

---

## Couple Dashboard (JWT-protected)

The couple manages photos at:

| Endpoint                                           | Description                          |
|----------------------------------------------------|--------------------------------------|
| `GET /weddings/{weddingId}/photos?status=pending`  | List photos, filter by status        |
| `PATCH /weddings/{weddingId}/photos/{id}/approve`  | Approve one photo                    |
| `PATCH /weddings/{weddingId}/photos/{id}/reject`   | Reject one photo                     |
| `DELETE /weddings/{weddingId}/photos/{id}`         | Delete one photo                     |
| `POST /weddings/{weddingId}/photos/bulk-approve`   | Approve multiple photos              |
| `POST /weddings/{weddingId}/photos/bulk-reject`    | Reject multiple photos               |
| `POST /weddings/{weddingId}/photos/bulk-delete`    | Delete multiple photos               |

All moderation endpoints require `wedding:edit` permission scoped to the wedding's tenant. The `weddingId` in the URL is always validated against the photo's actual `WeddingId` to prevent cross-wedding access (IDOR protection).

---

## Subscription Plan Tiers

Photo limits are enforced by the wedding's subscription plan. The plan is stored as an integer column on the `profiles` table (`WeddingPlan` enum, defaults to `Free`).

| Plan   | Photo Limit | Bulk Download |
|--------|-------------|---------------|
| Free   | 50          | No            |
| Pro    | 250         | No            |
| Plus   | Unlimited   | Yes           |

Limits are constants in `WeddingPlanLimits` in the Domain layer — no DB rows, no config keys. Upgrading a wedding's plan means updating `Wedding.Plan` via `ChangePlan()`, which the Subscriptions module will call via an integration event.

The photo limit check runs in two places:

1. **Presign step** — fast-path check before generating S3 URLs. If the requested count would exceed the limit, the request is rejected early with `GuestPhotoErrors.PlanLimitReached`.
2. **Register step** — fast-path check again before inserting, then a final enforcement query inside the insert transaction (`InsertManyAndEnforceAsync`) that marks any over-limit photos as `OverLimit` to handle concurrent upload races.

---

## Implementation Notes

### CDN Provider

`IPhotoStorageService` in the Application layer defines the contract. Two implementations exist:

- **`S3PhotoStorageService`** (registered in production) — uses `AWSSDK.S3` with `ForcePathStyle = true`, compatible with MinIO locally and Cloudflare R2 / AWS S3 in production. Generates presigned `PUT` URLs with a 15-minute expiry.
- **`StubPhotoStorageService`** — returns synthetic `https://stub.cdn/photos/{slotId}` URLs for offline local development without Docker.

`S3PhotoStorageService` is registered as a **Singleton** (the `AmazonS3Client` is thread-safe and expensive to construct).

**Docker / local dev:** MinIO runs on port `9000` (S3 API) and `9001` (console). The API communicates with MinIO via `http://eternelle.minio:9000` (internal Docker hostname). Presigned URLs are rewritten to `http://localhost:9000` before being returned to the browser — in production `ServiceUrl == PublicUrl` so the rewrite is a no-op.

**Production (Cloudflare R2):** Set `Weddings:PhotoStorage:ServiceUrl` to the R2 S3 endpoint used for API operations (presigning, uploads) and `PublicUrl` to the public-facing CDN domain used for serving images to clients (e.g. `https://cdn.example.com`). Cloudflare R2 exposes the S3 API — no code changes needed.

```json
// appsettings.Production.json (example R2 config)
{
  "Weddings": {
    "PhotoStorage": {
      "ServiceUrl": "https://<account-id>.r2.cloudflarestorage.com",
      "PublicUrl": "https://cdn.eternelle.ph",
      "AccessKey": "<r2-access-key>",
      "SecretKey": "<r2-secret-key>",
      "BucketName": "eternelle-photos"
    }
  }
}
```

### Upload Slot Store

`RedisUploadSlotStore` uses `IConnectionMultiplexer` directly (backed by the existing Redis instance). Slot TTL is 15 minutes. Redemption uses Redis `GETDEL` (`StringGetDeleteAsync`) which atomically returns and removes the key in a single round-trip — a slot can never be redeemed twice, even under concurrent requests.

### Why Redis for slots?

Without the slot store, any client knowing a CDN URL (e.g. by guessing or observing network traffic) could register arbitrary URLs as guest photos. The slot store enforces that only URLs the server generated can be registered — the client proves ownership by returning the `slotId` that was issued during presign.

### Known TODOs

- Thumbnail URLs — currently `null` on all registered photos. Thumbnails should be CDN-derived via URL transformation parameters (e.g. Cloudflare Image Resizing). No separate upload step needed.
- Bulk download endpoint (`GET /weddings/{weddingId}/photos/download`) — Plus tier only. Should zip all approved photos and return a signed download URL.
