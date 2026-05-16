namespace Eternelle.Modules.Weddings.Infrastructure.Storage;

public sealed class PhotoStorageOptions
{
    public const string SectionName = "Weddings:PhotoStorage";

    /// <summary>
    /// Internal endpoint used by the API to communicate with the storage service.
    /// In Docker Compose this is the Docker-internal hostname, e.g. http://eternelle.minio:9000.
    /// In production this is the same as <see cref="PublicUrl"/>.
    /// </summary>
    public string ServiceUrl { get; init; } = string.Empty;

    /// <summary>
    /// Publicly accessible base URL embedded in presigned URLs returned to the browser.
    /// In local development this differs from <see cref="ServiceUrl"/> because the browser
    /// cannot reach Docker-internal hostnames, e.g. http://localhost:9000.
    /// In production set this equal to <see cref="ServiceUrl"/>.
    /// </summary>
    public string PublicUrl { get; init; } = string.Empty;

    public string AccessKey { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public string BucketName { get; init; } = "eternelle-photos";
}
