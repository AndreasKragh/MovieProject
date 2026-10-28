namespace MovieVault.Api.Entities;

public class ProductionRecord
{
    public int ProductionRecordId { get; set; }

    public decimal Budget { get; set; }
    public DateOnly ProductionStartDate { get; set; }
    public DateOnly ProductionEndDate { get; set; }

    // 1:1 – FK med unikt index ligger her
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
}