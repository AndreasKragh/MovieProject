namespace MovieVault.Api.Entities;

public class Screening
{
    public int ScreeningId { get; set; }

    public DateOnly ScreeningDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public decimal Price { get; set; }

    // FK -> Movie (N:1)
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    // FK -> Screen (N:1)
    public int ScreenId { get; set; }
    public Screen Screen { get; set; } = null!;

    public ICollection<TicketSale> TicketSales { get; set; } = new List<TicketSale>();
}