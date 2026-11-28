using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

// Et medie (avis, tv-kanal osv.), som anmeldere arbejder for.
// Oplysningerne om mediet står kun her, så de ikke gentages på hver anmeldelse.
public class MediaOutlet
{
    public int MediaOutletId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // newspaper, tv, radio, magazine eller online (check constraint i DbContext)
    [MaxLength(20)]
    public string OutletType { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Website { get; set; }

    public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
