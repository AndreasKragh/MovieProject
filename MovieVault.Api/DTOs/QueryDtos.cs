namespace MovieVault.Api.Dtos;

// =====================================================================
// Query-endpoint 3: Upcoming screenings (sorteret kronologisk)
// =====================================================================

public record UpcomingScreeningDto(
    int ScreeningId,
    string MovieTitle,
    string CinemaName,
    int ScreenNumber,
    DateOnly ScreeningDate,
    TimeOnly StartTime,
    decimal Price);

// =====================================================================
// Query-endpoint 4: Ticket sales by movie
// =====================================================================

public record MovieTicketSalesDto(
    int MovieId,
    string MovieTitle,
    int TotalTicketsSold,
    decimal TotalRevenue);

// =====================================================================
// Query-endpoint 5: Actor movie performance
// =====================================================================

public record ActorMoviePerformanceDto(
    int ActorId,
    string ActorName,
    int TotalTicketsSold,
    decimal TotalRevenue,
    List<ActorMovieDto> Movies);

public record ActorMovieDto(
    int MovieId,
    string MovieTitle,
    string CharacterName,
    string RoleType,
    int TicketsSold,
    decimal Revenue);
