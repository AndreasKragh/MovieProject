using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Screen
{
    public int ScreenId { get; set; }

    public int ScreenNumber { get; set; }
    public int SeatingCapacity { get; set; }

    // '2D', '3D', 'IMAX' eller '4DX' (check constraint i DbContext)
    [MaxLength(20)]
    public string ScreenType { get; set; } = "2D";

    // FK -> Cinema (N:1)
    public int CinemaId { get; set; }
    public Cinema Cinema { get; set; } = null!;

    public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
}