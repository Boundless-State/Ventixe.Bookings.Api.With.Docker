using Microsoft.EntityFrameworkCore;
using Ventixe.Bookings.Api.Data;
using Ventixe.Bookings.Api.DTOs;
using Ventixe.Bookings.Api.Entities;

namespace Ventixe.Bookings.Api.Services;

public class BookingService
{
    private readonly BookingDbContext _context;

    public BookingService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllBookingsAsync()
    {
        var bookings = await _context.Bookings.ToListAsync();
        return bookings;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllBookingsAsync(string userId)
    {
        var bookings = await _context.Bookings.Where(x => x.UserId == userId).ToListAsync();
        return bookings;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllAsync(BookingFilterDto filter, bool isAdmin, string? userId)
    {
        var query = _context.Bookings.AsQueryable();

        if (!isAdmin && userId != null)
        {
            query = query.Where(q => q.UserId == userId);
        }

        if (filter.Status?.Any() == true)
        {
            query = query.Where(q => filter.Status.Contains(q.Status.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(q =>
                q.CustomerName.Contains(filter.Search) ||
                q.EventName.Contains(filter.Search) ||
                q.InvoiceId.Contains(filter.Search));
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(b => b.BookingDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(b => b.BookingDate <= filter.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            query = filter.SortBy switch
            {
                "BookingDate" => filter.SortDesc ? query.OrderByDescending(q => q.BookingDate) : query.OrderBy(q => q.BookingDate),
                "Price" => filter.SortDesc ? query.OrderByDescending(q => q.Price) : query.OrderBy(q => q.Price),
                "CustomerName" => filter.SortDesc ? query.OrderByDescending(q => q.CustomerName) : query.OrderBy(q => q.CustomerName),
                "InvoiceId" => filter.SortDesc ? query.OrderByDescending(q => q.InvoiceId) : query.OrderBy(q => q.InvoiceId),
                "Quantity" => filter.SortDesc ? query.OrderByDescending(q => q.Quantity) : query.OrderBy(q => q.Quantity),
                "Amount" => filter.SortDesc ? query.OrderByDescending(q => q.Price * q.Quantity) : query.OrderBy(q => q.Price * q.Quantity),
                "Status" => filter.SortDesc ? query.OrderByDescending(q => q.Status) : query.OrderBy(q => q.Status),
                "EVoucher" => filter.SortDesc ? query.OrderByDescending(q => q.EVoucher) : query.OrderBy(q => q.EVoucher),
                _ => query.OrderByDescending(q => q.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        query = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return await query.ToListAsync();
    }

    public async Task<BookingEntity?> GetByIdAsync(string bookingId)
    {
        return await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookingId);
    }

    public async Task<BookingEntity> CreateAsync(BookingEntity booking)
    {
        booking.BookingId = Guid.NewGuid().ToString();
        booking.CreatedAt = DateTime.UtcNow;

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    public async Task<bool> UpdateAsync(BookingEntity updatedBooking)
    {
        var existing = await _context.Bookings.FindAsync(updatedBooking.BookingId);
        if (existing == null)
            return false;

        existing.Status = updatedBooking.Status;
        existing.Quantity = updatedBooking.Quantity;
        existing.EVoucher = updatedBooking.EVoucher;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null)
            return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BookingStatsDto> GetStatisticsAsync()
    {
        var bookings = await _context.Bookings.ToListAsync();
        return new BookingStatsDto
        {
            TotalBookings = bookings.Count,
            TotalTicketsSold = bookings.Sum(b => b.Quantity),
            TotalEarnings = bookings.Sum(b => b.Price * b.Quantity)
        };
    }
}


