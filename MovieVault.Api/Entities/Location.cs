using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Location
{
    public int LocationId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Country { get; set; } = string.Empty;

    // 'street', 'building', 'landscape' eller 'set' (check constraint i DbContext)
    [MaxLength(50)]
    public string LocationType { get; set; } = string.Empty;

    public ICollection<FilmedAt> FilmedAt { get; set; } = new List<FilmedAt>();
}