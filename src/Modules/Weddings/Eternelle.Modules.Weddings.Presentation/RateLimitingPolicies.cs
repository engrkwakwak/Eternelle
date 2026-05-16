namespace Eternelle.Modules.Weddings.Presentation;

public static class RateLimitingPolicies
{
    /// <summary>
    /// Applied to the guest photo upload endpoint.
    /// Policy is configured in <c>Program.cs</c>: 10 requests / 60 s per IP.
    /// </summary>
    public const string GuestPhotoUpload = "guest-photo-upload";
}
