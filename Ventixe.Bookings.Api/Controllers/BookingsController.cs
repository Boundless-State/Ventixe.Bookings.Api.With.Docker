using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Ventixe.Bookings.Api.DTOs;
using Ventixe.Bookings.Api.Entities;
using Ventixe.Bookings.Api.Services;

namespace Ventixe.Bookings.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly BookingService _service;

    public BookingsController(BookingService service)
    {
        _service = service;
    }

    [SwaggerOperation(Summary = "Hämtar alla bokningar i systemet (endast Admin).")]
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-bookings")]
    public async Task<IActionResult> GetAllBookings([FromQuery] BookingFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter, true, null);
        return Ok(result);
    }

    [SwaggerOperation(Summary = "Hämtar alla bokningar för den inloggade medlemmen.")]
    [Authorize(Roles = "Member")]
    [HttpGet("user-bookings")]
    public async Task<IActionResult> GetUserBookings([FromQuery] BookingFilterDto filter)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null
            ? Unauthorized()
            : Ok(await _service.GetAllAsync(filter, false, userId));
    }

    [SwaggerOperation(Summary = "Hämtar en bokning med angivet ID (Admin eller ägare).")]
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var booking = await _service.GetByIdAsync(id);
        return booking is not null ? Ok(booking) : NotFound();
    }

    [SwaggerOperation(Summary = "Skapar en ny bokning (endast för inloggade medlemmar).")]
    [Authorize(Roles = "Member")]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        dto.UserId = userId;
        dto.CustomerName = User.Identity?.Name ?? "Member";

        var booking = new BookingEntity
        {
            InvoiceId = dto.InvoiceId,
            BookingDate = dto.BookingDate,
            UserId = dto.UserId,
            CustomerName = dto.CustomerName,
            EventId = dto.EventId,
            EventName = dto.EventName,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            TicketCategoryId = dto.TicketCategoryId,
            TicketCategoryName = dto.TicketCategoryName,
            Price = dto.Price,
            Quantity = dto.Quantity,
            EVoucher = dto.EVoucher,
            Status = BookingStatus.Pending
        };

        var created = await _service.CreateAsync(booking);
        return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, created);
    }

    [SwaggerOperation(Summary = "Uppdaterar en bokning (status, antal, e-voucher).")]
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(string id, [FromBody] UpdateBookingDto dto)
    {
        var updated = new BookingEntity
        {
            BookingId = id,
            Status = dto.Status,
            Quantity = dto.Quantity,
            EVoucher = dto.EVoucher
        };

        return await _service.UpdateAsync(updated)
            ? NoContent()
            : NotFound();
    }

    [SwaggerOperation(Summary = "Tar bort en bokning (endast för Admin)")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        return await _service.DeleteAsync(id)
            ? NoContent()
            : NotFound();
    }

    [SwaggerOperation(Summary = "Avbokar en bokning för inloggad medlem (ändrar status till Cancelled)")]
    [Authorize(Roles = "Member")]
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> MemberCancelBooking(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var booking = await _service.GetByIdAsync(id);
        if (booking == null || booking.UserId != userId)
            return NotFound();

        booking.Status = BookingStatus.Cancelled;
        var result = await _service.UpdateAsync(booking);

        return result ? Ok() : BadRequest();
    }

    [SwaggerOperation(Summary = "Hämtar statistik över bokningar (antal, biljetter och intäkter)")]
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _service.GetStatisticsAsync();
        return Ok(stats);
    }

}
