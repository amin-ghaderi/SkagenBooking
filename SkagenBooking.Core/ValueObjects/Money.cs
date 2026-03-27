namespace SkagenBooking.Core.ValueObjects;

/// <summary>
/// Represents a monetary value.
/// </summary>
public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}