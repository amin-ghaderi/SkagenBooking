namespace SkagenBooking.Core.ValueObjects;

/// <summary>
/// Value object representing a monetary amount in a specific currency.
/// </summary>
public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money operator *(Money money, int multiplier)
    {
        if (multiplier < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier));
        }

        return new Money(money.Amount * multiplier, money.Currency);
    }
}