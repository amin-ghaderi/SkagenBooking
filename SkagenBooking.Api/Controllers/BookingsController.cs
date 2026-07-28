using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkagenBooking.Api.Contracts.Bookings;
using SkagenBooking.Api.Mapping;
using SkagenBooking.Application.Bookings.Commands.CancelBooking;
using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Commands.UpdateBooking;
using SkagenBooking.Application.Bookings.Queries.GetBookings;

namespace SkagenBooking.Api.Controllers;

/// <summary>
/// Manages booking lifecycle: create, list, update, and cancel.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class BookingsController : ControllerBase
{
    /// <summary>
    /// Creates a new booking.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        [FromBody] CreateBookingRequest request,
        [FromServices] ICreateBookingUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var command = new CreateBookingCommand
        {
            UserId = userId,
            PropertyId = request.PropertyId,
            RoomId = request.RoomId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            GuestCount = request.GuestCount,
            NeedsParking = request.NeedsParking,
            IsLateArrival = request.IsLateArrival,
            EstimatedArrivalTime = request.EstimatedArrivalTime
        };

        var result = await useCase.ExecuteAsync(command, cancellationToken);
        if (!result.IsCreated)
        {
            return BadRequest(new { message = result.Message });
        }

        if (result.BookingId is null || result.TotalAmount is null)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Booking was created but response was incomplete.");
        }

        var response = new CreateBookingResponse
        {
            Id = result.BookingId.Value,
            TotalAmount = result.TotalAmount.Value,
            Currency = result.Currency,
            Message = result.Message
        };

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Lists bookings owned by the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(
        [FromServices] IGetBookingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var bookings = await useCase.ExecuteAsync(
            new GetBookingsQuery { UserId = userId, PropertyId = null },
            cancellationToken);
        var response = bookings.Select(BookingResponseMapper.From).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Gets a single booking by id when owned by the authenticated user.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById(
        [FromRoute] int id,
        [FromServices] IGetBookingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var bookings = await useCase.ExecuteAsync(
            new GetBookingsQuery { UserId = userId, PropertyId = null },
            cancellationToken);
        var booking = bookings.FirstOrDefault(b => b.Id == id);
        if (booking is null)
        {
            return NotFound();
        }

        return Ok(BookingResponseMapper.From(booking));
    }

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateBookingRequest request,
        [FromServices] IUpdateBookingUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var command = new UpdateBookingCommand
        {
            UserId = userId,
            BookingId = id,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            GuestCount = request.GuestCount,
            NeedsParking = request.NeedsParking,
            IsLateArrival = request.IsLateArrival,
            EstimatedArrivalTime = request.EstimatedArrivalTime
        };

        var result = await useCase.ExecuteAsync(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(new { message = "Booking updated." });
        }

        return result.Error switch
        {
            UpdateBookingError.NotFound => NotFound(new { message = result.Message }),
            UpdateBookingError.Conflict => Conflict(new { message = result.Message }),
            _ => BadRequest(new { message = result.Message })
        };
    }

    /// <summary>
    /// Cancels a booking.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Cancel(
        [FromRoute] int id,
        [FromServices] ICancelBookingUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await useCase.ExecuteAsync(
            new CancelBookingCommand { UserId = userId, BookingId = id },
            cancellationToken);
        if (result.IsSuccess)
        {
            return NoContent();
        }

        if (result.Error == CancelBookingError.NotFound)
        {
            return NotFound(new { message = result.Message });
        }

        return BadRequest(new { message = result.Message });
    }

    private string? GetRequiredUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
