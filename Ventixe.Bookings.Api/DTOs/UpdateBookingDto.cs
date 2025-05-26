using Ventixe.Bookings.Api.Entities;

namespace Ventixe.Bookings.Api.DTOs;

public class UpdateBookingDto
{
    public BookingStatus Status { get; set; }
    public int Quantity { get; set; }
    public string? EVoucher { get; set; }
}
