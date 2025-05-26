using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ventixe.Bookings.Api.Entities;

[Table("Bookings")]
public class BookingEntity
{
    [Key]
    [Required]
    public string BookingId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string InvoiceId { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "date")]
    public DateTime BookingDate { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string CustomerName { get; set; } = null!;

    [Required]
    public string EventId { get; set; } = null!;

    [Required]
    public string EventName { get; set; } = null!;


    [Required]
    public string CategoryId { get; set; } = null!;

    [Required]
    public string CategoryName { get; set; } = null!;


    [Required]
    public string TicketCategoryId { get; set; } = null!;

    [Required]
    public string TicketCategoryName { get; set; } = null!;


    [Column(TypeName = "money")]
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    //Jag frågade gpt om det var okej att ha en property som inte är i databasen,
    //och den sa att det var okej att inte ha det med att skriva [NotMapped],
    //eftersom det bara är en uträkning av totalen i bokningen.
    //Så jag har lagt till det här, och det är en decimal som multiplicerar priset med kvantiteten.
    [NotMapped]
    public decimal TotalAmount => Price * Quantity;


    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? EVoucher { get; set; }
}

