using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SkagenBooking.Tests.Integration;

public sealed class SkagenBookingApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"skagenbooking-test-{Guid.NewGuid():N}.db");
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={databasePath}");
    }
}
