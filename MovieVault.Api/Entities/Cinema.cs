using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Cinema
{
    public int CinemaId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    public int OpeningYear { get; set; }

    public ICollection<Screen> Screens { get; set; } = new List<Screen>();
}