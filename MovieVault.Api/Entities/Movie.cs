// Entities/Movie.cs
namespace MovieVault.Api.Entities;

public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; } = "";
    public int Runtime { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string AgeRating { get; set; } = "";
    public string Status { get; set; } = "";

    public int? DirectorId { get; set; }          // foreign key, nullable = optional
    public Director? Director { get; set; }       // navigation property

    public ProductionRecord? ProductionRecord { get; set; }               // 1:1
    public ICollection<MovieCast> Cast { get; set; } = new List<MovieCast>(); // N:M
}