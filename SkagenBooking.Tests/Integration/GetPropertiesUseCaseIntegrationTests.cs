using SkagenBooking.Application.Properties.Queries.GetProperties;
using SkagenBooking.Infrastructure.Repositories;

namespace SkagenBooking.Tests.Integration;

public class GetPropertiesUseCaseIntegrationTests
{
    [Fact]
    public async Task GetProperties_Should_Return_Seeded_Property()
    {
        var useCase = new GetPropertiesUseCase(new InMemoryPropertyRepository());

        var properties = await useCase.ExecuteAsync(new GetPropertiesQuery(), CancellationToken.None);

        Assert.Single(properties);
        Assert.Equal(1, properties[0].Id);
        Assert.Equal("Pernille's Bed & Breakfast", properties[0].Name);
        Assert.Equal("Skagen", properties[0].City);
    }
}
