using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Dtos;

// =====================================================================
// REQUEST
// =====================================================================

/// <summary>
/// Bruges til både POST (opret) og PUT (opdater) af en screening.
/// En screening skal altid referere til en film og en sal.
/// </summary>
public class ScreeningWriteDto
{
    [Range(1, int.MaxValue, ErrorMessage = "MovieId must be a positive number.")]
    public int MovieId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ScreenId must be a positive number.")]
    public int ScreenId { get; set; }

    [Required]
    public DateOnly ScreeningDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Range(typeof(decimal), "0", "100000", ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }
}

// =====================================================================
// RESPONSE
// =====================================================================

/// <summary>
/// Bruges til "screenings for en film" og "screenings for en biograf".
/// SeatsAvailable = SeatingCapacity - TicketsSold.
/// </summary>
public record ScreeningDto(
    int ScreeningId,
    int MovieId,
    string MovieTitle,
    int CinemaId,
    string CinemaName,
    int ScreenId,
    int ScreenNumber,
    string ScreenType,
    DateOnly ScreeningDate,
    TimeOnly StartTime,
    decimal Price,
    int SeatingCapacity,
    int TicketsSold,
    int SeatsAvailable);
