using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Dtos;

// =====================================================================
// REQUESTS (data ind i API'en)
// =====================================================================

/// <summary>Bruges til både POST (opret) og PUT (opdater) af en film.</summary>
public class MovieWriteDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 600, ErrorMessage = "Runtime must be between 1 and 600 minutes.")]
    public int Runtime { get; set; }

    [Required]
    public DateOnly ReleaseDate { get; set; }

    [Required, StringLength(10)]
    public string AgeRating { get; set; } = string.Empty;

    [Required]
    [AllowedValues("in production", "released", "archived", "planned",
        ErrorMessage = "MovieStatus must be one of: in production, released, archived, planned.")]
    public string MovieStatus { get; set; } = "planned";

    // Valgfri: en film kan oprettes før instruktør/selskab er kendt
    public int? DirectorId { get; set; }
    public int? ProductionCompanyId { get; set; }

    // Valgfri: produktionsdata (1:0..1)
    public ProductionRecordInputDto? ProductionRecord { get; set; }
}

public class ProductionRecordInputDto : IValidatableObject
{
    [Range(typeof(decimal), "0", "100000000000", ErrorMessage = "Budget cannot be negative.")]
    public decimal Budget { get; set; }

    [Required]
    public DateOnly ProductionStartDate { get; set; }

    [Required]
    public DateOnly ProductionEndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ProductionEndDate < ProductionStartDate)
        {
            yield return new ValidationResult(
                "ProductionEndDate cannot be before ProductionStartDate.",
                new[] { nameof(ProductionEndDate) });
        }
    }
}

/// <summary>POST /api/movies/{id}/cast</summary>
public class AssignActorDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ActorId must be a positive number.")]
    public int ActorId { get; set; }

    [Required, StringLength(100, MinimumLength = 1)]
    public string CharacterName { get; set; } = string.Empty;

    [Required]
    [AllowedValues("lead", "supporting", "cameo",
        ErrorMessage = "RoleType must be one of: lead, supporting, cameo.")]
    public string RoleType { get; set; } = string.Empty;
}

/// <summary>POST /api/movies/{id}/locations</summary>
public class AddFilmingLocationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "LocationId must be a positive number.")]
    public int LocationId { get; set; }

    [Range(1, 3650, ErrorMessage = "NumberOfShootingDays must be at least 1.")]
    public int NumberOfShootingDays { get; set; }
}

// =====================================================================
// RESPONSES (data ud af API'en)
// =====================================================================

/// <summary>GET /api/movies – oversigt</summary>
public record MovieOverviewDto(
    int MovieId,
    string Title,
    DateOnly ReleaseDate,
    string MovieStatus,
    string? DirectorName,
    string? ProductionCompanyName,
    decimal? ProductionBudget);

/// <summary>GET /api/movies/{id} – detaljer</summary>
public record MovieDetailsDto(
    int MovieId,
    string Title,
    int Runtime,
    DateOnly ReleaseDate,
    string AgeRating,
    string MovieStatus,
    DirectorDto? Director,
    ProductionCompanyDto? ProductionCompany,
    ProductionRecordDto? ProductionRecord,
    List<CastMemberDto> Cast,
    List<FilmingLocationDto> FilmingLocations);

public record DirectorDto(
    int DirectorId,
    string Name,
    DateOnly BirthDate,
    string Nationality);

public record ProductionCompanyDto(
    int ProductionCompanyId,
    string Name,
    int FoundedYear,
    string Country);

public record ProductionRecordDto(
    decimal Budget,
    DateOnly ProductionStartDate,
    DateOnly ProductionEndDate);

public record CastMemberDto(
    int ActorId,
    string ActorName,
    string CharacterName,
    string RoleType);

public record FilmingLocationDto(
    int LocationId,
    string LocationName,
    string City,
    string Country,
    string LocationType,
    int NumberOfShootingDays);
