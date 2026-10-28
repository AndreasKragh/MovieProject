using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Studio
{
    public int StudioId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Country { get; set; } = string.Empty;

    public int NumberOfSoundStages { get; set; }
    public int Capacity { get; set; }

    // FK -> ProductionCompany (N:1)
    public int CompanyId { get; set; }
    public ProductionCompany ProductionCompany { get; set; } = null!;
}