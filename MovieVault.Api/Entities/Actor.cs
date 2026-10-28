using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Actor
{
    public int ActorId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    [MaxLength(50)]
    public string Nationality { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    public ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
}