namespace MovieVault.Api.Entities;

public class TicketSale
{
    public int TicketSaleId { get; set; }

    public DateTime SaleDateTime { get; set; }

    // Antal billetter i denne transaktion (> 0)
    public int Quantity { get; set; }

    // Samlet pris for hele salget (>= 0)
    public decimal TotalPrice { get; set; }

    // FK -> Screening (N:1)
    public int ScreeningId { get; set; }
    public Screening Screening { get; set; } = null!;
}