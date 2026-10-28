using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class ProductionCompany
{
    public int ProductionCompanyId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int FoundedYear { get; set; }

    [MaxLength(50)]
    public string Country { get; set; } = string.Empty;

    public ICollection<Studio> Studios { get; set; } = new List<Studio>();
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}