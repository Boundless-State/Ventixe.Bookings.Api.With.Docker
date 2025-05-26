using Microsoft.EntityFrameworkCore;
using Ventixe.Bookings.Api.Entities;

namespace Ventixe.Bookings.Api.Data;

public class BookingDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<BookingEntity> Bookings { get; set; }
}
