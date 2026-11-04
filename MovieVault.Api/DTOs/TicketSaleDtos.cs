using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Dtos;

// =====================================================================
// REQUESTS
// =====================================================================

/// <summary>
/// POST /api/ticketsales
/// TotalPrice sendes ikke af klienten: serveren beregner Quantity * Screening.Price.
/// </summary>
public class CreateTicketSaleDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ScreeningId must be a positive number.")]
    public int ScreeningId { get; set; }

    [Range(1, 1000, ErrorMessage = "Quantity must be at least 1 (zero or negative is not allowed).")]
    public int Quantity { get; set; }
}

/// <summary>
/// PUT /api/ticketsales/{id}
/// Kun antallet kan ændres. Serveren genberegner TotalPrice og tjekker kapaciteten igen.
/// </summary>
public class UpdateTicketSaleDto
{
    [Range(1, 1000, ErrorMessage = "Quantity must be at least 1 (zero or negative is not allowed).")]
    public int Quantity { get; set; }
}

// =====================================================================
// RESPONSE
// =====================================================================

public record TicketSaleDto(
    int TicketSaleId,
    int ScreeningId,
    string MovieTitle,
    string CinemaName,
    int ScreenNumber,
    DateOnly ScreeningDate,
    TimeOnly StartTime,
    DateTime SaleDateTime,
    int Quantity,
    decimal TotalPrice);
