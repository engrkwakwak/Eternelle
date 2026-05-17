using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

/// <summary>
/// Free-text label describing the type of payment account (e.g. "GCash",
/// "BPI Savings", "PayPal"). Up to 100 characters.
/// </summary>
public sealed record AccountType
{
    public const int MaxLength = 100;

    private AccountType(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<AccountType> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<AccountType>(AccountTypeErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<AccountType>(AccountTypeErrors.TooLong);
        }

        return Result.Success(new AccountType(trimmed));
    }

    public override string ToString() => Value;

    internal static AccountType FromPersistence(string value) => new(value);
}
