// Entities/Movie.cs
namespace MovieVault.Api.Entities;

public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; } = "";
    public int Runtime { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string AgeRating { get; set; } = "";
    public string MovieStatus { get; set; } = "planned";  //Standardværdi er planned for at overholde constraint

    public int? DirectorId { get; set; }          // foreign key, nullable = optional
    public int ProductionCompanyId { get; set; } 
    public Director? Director { get; set; }       // navigation property

    public ProductionRecord? ProductionRecord { get; set; }               // 1:1

    public ProductionCompany? ProductionCompany { get; set; }             // 1:N
    public ICollection<MovieCast> Cast { get; set; } = new List<MovieCast>(); // N:M
    public ICollection<FilmedAt> FilmingLocations { get; set; } = new List<FilmedAt>(); // N:M
    public ICollection<Screening> Screenings { get; set; } = new List<Screening>(); // 1:N
}
