namespace Eternelle.Common.Domain.ValueObjects;

/// <summary>
/// Value object representing an external web URL (e.g. vendor website, gift registry link).
/// Must be an absolute http or https URI — relative paths are rejected because these
/// values are rendered as user-facing outbound links.
/// </summary>
public sealed record WebUrl
{
    public static readonly int MaxLength = 2048;

    private WebUrl(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<WebUrl> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<WebUrl>(WebUrlErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<WebUrl>(WebUrlErrors.TooLong);
        }

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? uri) ||
            uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            return Result.Failure<WebUrl>(WebUrlErrors.InvalidFormat);
        }

        return Result.Success(new WebUrl(trimmed));
    }

    public override string ToString() => Value;

    public static WebUrl FromPersistence(string value) => new(value);
}
