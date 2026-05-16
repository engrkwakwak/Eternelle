using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

/// <summary>
/// Bank account, mobile-wallet number, or similar payment identifier shown
/// on a gift option. Limited to digits, dashes, and spaces — the surface for
/// every supported gateway. Up to 50 characters.
/// </summary>
public sealed record AccountNumber
{
    public static readonly int MaxLength = 50;

    private AccountNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<AccountNumber> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<AccountNumber>(AccountNumberErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<AccountNumber>(AccountNumberErrors.TooLong);
        }

        if (!trimmed.All(c => char.IsDigit(c) || c == '-' || c == ' '))
        {
            return Result.Failure<AccountNumber>(AccountNumberErrors.InvalidFormat);
        }

        return Result.Success(new AccountNumber(trimmed));
    }

    public override string ToString() => Value;

    internal static AccountNumber FromPersistence(string value) => new(value);
}
