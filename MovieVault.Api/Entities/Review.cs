using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Review
{
    public int ReviewId { get; set; }

    [MaxLength(200)]
    public string Headline { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    // 1-5 stjerner (check constraint i DbContext)
    public int Rating { get; set; }

    // Sættes af serveren ved oprettelse (UTC)
    public DateTime PublishedAt { get; set; }

    // FK -> Movie (N:1). En film kan have 0..N anmeldelser.
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    // FK -> Reviewer (N:1): forfatteren
    public int ReviewerId { get; set; }
    public Reviewer Reviewer { get; set; } = null!;

    // FK -> MediaOutlet (N:1): det medie, anmeldelsen blev udgivet i.
    // Kopieres fra anmelderen ved oprettelse. Gemmes på anmeldelsen, fordi en anmelder
    // kan skifte medie senere, mens en udgivet anmeldelse stadig hører til det oprindelige.
    public int MediaOutletId { get; set; }
    public MediaOutlet MediaOutlet { get; set; } = null!;
}
