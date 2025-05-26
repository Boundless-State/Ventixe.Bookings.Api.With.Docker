namespace Ventixe.Bookings.Api.DTOs;

public class CreateBookingDto
{
    public string InvoiceId { get; set; } = null!;
    public DateTime BookingDate { get; set; }

    public string UserId { get; set; } = null!;
    public string CustomerName { get; set; } = null!;

    public string EventId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public string CategoryName { get; set; } = null!;

    public string TicketCategoryId { get; set; } = null!;
    public string TicketCategoryName { get; set; } = null!;

    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public string? EVoucher { get; set; }
}
