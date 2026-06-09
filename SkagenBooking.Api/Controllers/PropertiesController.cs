using Microsoft.AspNetCore.Mvc;
using SkagenBooking.Api.Contracts.Properties;
using SkagenBooking.Api.Mapping;
using SkagenBooking.Application.Properties.Queries.GetProperties;

namespace SkagenBooking.Api.Controllers;

/// <summary>
/// Provides read access to bookable properties.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class PropertiesController : ControllerBase
{
    /// <summary>
    /// Lists all properties available for booking.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PropertyResponse>>> GetAll(
        [FromServices] IGetPropertiesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var properties = await useCase.ExecuteAsync(new GetPropertiesQuery(), cancellationToken);
        var response = properties.Select(PropertyResponseMapper.From).ToList();
        return Ok(response);
    }
}
