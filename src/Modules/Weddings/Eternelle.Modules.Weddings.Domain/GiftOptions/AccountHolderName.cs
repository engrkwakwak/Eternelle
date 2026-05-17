using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

/// <summary>
/// Name of the bank/e-wallet account holder displayed on a gift option.
/// Free text up to 100 characters.
/// </summary>
public sealed record AccountHolderName
{
    public const int MaxLength = 100;

    private AccountHolderName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<AccountHolderName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<AccountHolderName>(AccountHolderNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<AccountHolderName>(AccountHolderNameErrors.TooLong);
        }

        return Result.Success(new AccountHolderName(trimmed));
    }

    public override string ToString() => Value;

    internal static AccountHolderName FromPersistence(string value) => new(value);
}
