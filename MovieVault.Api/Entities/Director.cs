// Entities/Director.cs
namespace MovieVault.Api.Entities;

public class Director
{
    public int DirectorId { get; set; }
    public string Name { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string Nationality { get; set; } = "";

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();   // "mange"-siden
}