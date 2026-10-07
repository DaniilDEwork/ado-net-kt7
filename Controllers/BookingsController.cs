using Kt7Api.Data;
using Kt7Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kt7Api.Controllers;

[ApiController]
[Route("bookings")]
public class BookingsController(AppStore store) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        lock (store.SyncRoot)
        {
            return Ok(store.Bookings.ToArray());
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        lock (store.SyncRoot)
        {
            var booking = store.Bookings.FirstOrDefault(item => item.Id == id);
            return booking is null ? NotFound(new { message = "Бронирование не найдено" }) : Ok(booking);
        }
    }

    [HttpPost]
    public IActionResult Create(BookingRequest request)
    {
        lock (store.SyncRoot)
        {
            if (!store.Resources.Any(item => item.Id == request.ResourceId))
            {
                return NotFound(new { message = "Ресурс не найден" });
            }
            if (store.HasConflict(request.ResourceId!.Value, request.Start!.Value, request.End!.Value))
            {
                return Conflict(new { message = "Ресурс уже забронирован на это время" });
            }
            var booking = MakeBooking(store.NextBookingId++, request);
            store.Bookings.Add(booking);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, BookingRequest request)
    {
        lock (store.SyncRoot)
        {
            var index = store.Bookings.FindIndex(item => item.Id == id);
            if (index == -1)
            {
                return NotFound(new { message = "Бронирование не найдено" });
            }
            if (!store.Resources.Any(item => item.Id == request.ResourceId))
            {
                return NotFound(new { message = "Ресурс не найден" });
            }
            if (store.HasConflict(request.ResourceId!.Value, request.Start!.Value, request.End!.Value, id))
            {
                return Conflict(new { message = "Ресурс уже забронирован на это время" });
            }
            var booking = MakeBooking(id, request);
            store.Bookings[index] = booking;
            return Ok(booking);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Cancel(int id)
    {
        lock (store.SyncRoot)
        {
            var removed = store.Bookings.RemoveAll(item => item.Id == id);
            return removed == 0 ? NotFound(new { message = "Бронирование не найдено" }) : NoContent();
        }
    }

    private static Booking MakeBooking(int id, BookingRequest request)
    {
        return new Booking
        {
            Id = id, ResourceId = request.ResourceId!.Value, CustomerName = request.CustomerName.Trim(),
            Start = request.Start!.Value, End = request.End!.Value
        };
    }
}
