using System.Net;
using System.Net.Http.Json;
using SkagenBooking.Api.Contracts.Properties;
using SkagenBooking.Api.Contracts.Rooms;

namespace SkagenBooking.Tests.Integration;

public class ApiReferenceDataEndpointsTests : IClassFixture<SkagenBookingApiFactory>
{
    private readonly HttpClient _client;

    public ApiReferenceDataEndpointsTests(SkagenBookingApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProperties_Should_Return_Seeded_Property()
    {
        var response = await _client.GetAsync("/api/properties");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var properties = await response.Content.ReadFromJsonAsync<IReadOnlyList<PropertyResponse>>();
        Assert.NotNull(properties);
        Assert.NotEmpty(properties);
        Assert.Contains(properties, p => p.Id == 1 && p.City == "Skagen");
    }

    [Fact]
    public async Task GetRooms_Should_Return_Rooms_With_Nightly_Rates()
    {
        var response = await _client.GetAsync("/api/rooms?propertyId=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rooms = await response.Content.ReadFromJsonAsync<IReadOnlyList<RoomResponse>>();
        Assert.NotNull(rooms);
        Assert.Equal(4, rooms.Count);
        Assert.Contains(rooms, r => r.Id == 1 && r.NightlyRate == 550m && r.Currency == "DKK");
    }

    [Fact]
    public async Task Options_Preflight_From_Vite_Origin_Should_Include_Cors_Headers()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/bookings");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await _client.SendAsync(request);

        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.Equal("http://localhost:5173", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }
}
