namespace MovieVault.Api.Dtos;

// Små læse-DTO'er, så man kan finde de ID'er, man skal bruge i andre requests
// (f.eks. ActorId til at tilknytte en skuespiller, ScreenId til at oprette en screening).
// DirectorDto og ProductionCompanyDto ligger allerede i MovieDtos.cs.

public record ActorDto(
    int ActorId,
    string Name,
    DateOnly BirthDate,
    string Nationality,
    string Gender);

public record LocationDto(
    int LocationId,
    string Name,
    string City,
    string Country,
    string LocationType);

public record ScreenDto(
    int ScreenId,
    int ScreenNumber,
    int SeatingCapacity,
    string ScreenType);

public record CinemaDto(
    int CinemaId,
    string Name,
    string Address,
    string City,
    int OpeningYear,
    List<ScreenDto> Screens);
