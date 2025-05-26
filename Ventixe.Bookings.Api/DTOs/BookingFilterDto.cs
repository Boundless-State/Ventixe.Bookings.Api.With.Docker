namespace Ventixe.Bookings.Api.DTOs;

public class BookingFilterDto
{
    public string? Search { get; set; }
    public List<string>? Status { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string? CategoryId { get; set; }
    public string? Month { get; set; }

    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

