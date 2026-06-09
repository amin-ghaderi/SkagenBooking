using Microsoft.AspNetCore.Mvc;
using SkagenBooking.Api.Contracts.Rooms;
using SkagenBooking.Api.Mapping;
using SkagenBooking.Application.Rooms.Queries.GetRooms;

namespace SkagenBooking.Api.Controllers;

/// <summary>
/// Provides read access to bookable rooms.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class RoomsController : ControllerBase
{
    /// <summary>
    /// Lists rooms, optionally filtered by property.
    /// </summary>
    /// <param name="propertyId">When set, returns only rooms for this property.</param>
    /// <param name="useCase">Room query use case.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoomResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAll(
        [FromQuery] int? propertyId,
        [FromServices] IGetRoomsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var rooms = await useCase.ExecuteAsync(new GetRoomsQuery { PropertyId = propertyId }, cancellationToken);
        var response = rooms.Select(RoomResponseMapper.From).ToList();
        return Ok(response);
    }
}
