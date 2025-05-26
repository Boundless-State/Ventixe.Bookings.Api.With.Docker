using Microsoft.EntityFrameworkCore;
using Ventixe.Bookings.Api.Data;
using Ventixe.Bookings.Api.Entities;

namespace Ventixe.Bookings.Api.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<BookingEntity>> GetAllAsync();
    Task<IEnumerable<BookingEntity>> GetBookingsByUserIdAsync(string userId);
    Task<BookingEntity?> GetBookingByIdAsync(string bookingId);
    Task AddBookingAsync(BookingEntity booking);
    Task UpdateBookingAsync(BookingEntity booking);
    Task DeleteBookingAsync(string bookingId);
}

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;
    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllAsync()
    {
        return await _context.Bookings.ToListAsync();
    }

    // Använder denna metod för framtiden, ifall vi behöver funktionaliteten
    // för att hämta bokningar för member som är inloggad (om vi vill skapa olika roles)
    public async Task<IEnumerable<BookingEntity>> GetBookingsByUserIdAsync(string userId)
    {
        return await _context.Bookings.Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<BookingEntity?> GetBookingByIdAsync(string bookingId)
    {
        return await _context.Bookings.FirstOrDefaultAsync
            (b => b.BookingId == bookingId);
    }

    public async Task AddBookingAsync(BookingEntity booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBookingAsync(BookingEntity booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookingAsync(string bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}