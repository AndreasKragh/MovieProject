using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Dtos;

// =====================================================================
// REQUESTS
// =====================================================================

/// <summary>
/// POST /api/reviews
/// Forfatter, medie og udgivelsestidspunkt sendes ikke: serveren bestemmer dem.
/// ReviewerId bruges kun af Admin til at oprette en anmeldelse for en bestemt anmelder.
/// </summary>
public class CreateReviewDto
{
    [Range(1, int.MaxValue, ErrorMessage = "MovieId must be a positive number.")]
    public int MovieId { get; set; }

    [Required, MaxLength(200)]
    public string Headline { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    /// <summary>Kun for Admin. En Reviewer kan ikke oprette anmeldelser for andre.</summary>
    public int? ReviewerId { get; set; }
}

/// <summary>
/// PUT /api/reviews/{id}
/// Kun indholdet kan ændres. Film, forfatter, medie og udgivelsestidspunkt ligger fast.
/// </summary>
public class UpdateReviewDto
{
    [Required, MaxLength(200)]
    public string Headline { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Body { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }
}

// =====================================================================
// RESPONSES
// =====================================================================

public record ReviewDto(
    int ReviewId,
    string Headline,
    string Body,
    int Rating,
    DateTime PublishedAt,
    int MovieId,
    string MovieTitle,
    int ReviewerId,
    string AuthorName,
    int MediaOutletId,
    string MediaOutletName,
    string MediaOutletType);

public record MediaOutletDto(
    int MediaOutletId,
    string Name,
    string OutletType,
    string? Website);
