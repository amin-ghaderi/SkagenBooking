using Microsoft.AspNetCore.Mvc;
using SkagenBooking.Api.Contracts.Bookings;
using SkagenBooking.Application.Bookings.Commands.CreateBooking;
using SkagenBooking.Application.Bookings.Queries.GetBookings;

namespace SkagenBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        [FromBody] CreateBookingRequest request,
        [FromServices] ICreateBookingUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookingCommand
        {
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
            BookingId = result.BookingId.Value,
            TotalAmount = result.TotalAmount.Value,
            Currency = result.Currency,
            Message = result.Message
        };

        return CreatedAtAction(nameof(GetById), new { id = response.BookingId }, response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(
        [FromServices] IGetBookingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var bookings = await useCase.ExecuteAsync(new GetBookingsQuery { PropertyId = null }, cancellationToken);

        var response = bookings
            .Select(b => new BookingResponse
            {
                Id = b.Id,
                PropertyId = b.PropertyId,
                RoomId = b.RoomId,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                GuestCount = b.GuestCount,
                NeedsParking = b.NeedsParking
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingResponse>> GetById(
        [FromRoute] int id,
        [FromServices] IGetBookingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var bookings = await useCase.ExecuteAsync(new GetBookingsQuery { PropertyId = null }, cancellationToken);
        var booking = bookings.FirstOrDefault(b => b.Id == id);
        if (booking is null)
        {
            return NotFound();
        }

        return Ok(new BookingResponse
        {
            Id = booking.Id,
            PropertyId = booking.PropertyId,
            RoomId = booking.RoomId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            GuestCount = booking.GuestCount,
            NeedsParking = booking.NeedsParking
        });
    }
}

