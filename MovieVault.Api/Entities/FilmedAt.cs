namespace MovieVault.Api.Entities;

// Join-entitet for N:M mellem Movie og Location (sammensat PK: MovieId + LocationId)
public class FilmedAt
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int NumberOfShootingDays { get; set; }
}