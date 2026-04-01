using SkagenBooking.Core.ValueObjects;

namespace SkagenBooking.Tests.Domain;

public class DateRangeTests
{
    [Fact]
    public void Overlaps_Should_Return_True_For_Intersecting_Ranges()
    {
        var first = new DateRange(new DateTime(2026, 4, 10), new DateTime(2026, 4, 15));
        var second = new DateRange(new DateTime(2026, 4, 14), new DateTime(2026, 4, 20));

        Assert.True(first.Overlaps(second));
    }

    [Fact]
    public void Overlaps_Should_Return_False_For_BackToBack_Ranges()
    {
        var first = new DateRange(new DateTime(2026, 4, 10), new DateTime(2026, 4, 15));
        var second = new DateRange(new DateTime(2026, 4, 15), new DateTime(2026, 4, 20));

        Assert.False(first.Overlaps(second));
    }
}
