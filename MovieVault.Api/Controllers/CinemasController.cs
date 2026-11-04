using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

// Kun læsning: opgaven kræver ikke CRUD for biografer og sale,
// men vi skal kunne hente screenings for en biograf.
[ApiController]
[Route("api/[controller]")]
public class CinemasController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public CinemasController(MovieVaultContext context)
    {
        _context = context;
    }

    private static readonly Expression<Func<Cinema, CinemaDto>> ToDto = c => new CinemaDto(
        c.CinemaId,
        c.Name,
        c.Address,
        c.City,
        c.OpeningYear,
        c.Screens
            .OrderBy(s => s.ScreenNumber)
            .Select(s => new ScreenDto(s.ScreenId, s.ScreenNumber, s.SeatingCapacity, s.ScreenType))
            .ToList());

    // GET /api/cinemas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CinemaDto>>> GetCinemas()
    {
        var cinemas = await _context.Cinemas
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(ToDto)
            .ToListAsync();

        return Ok(cinemas);
    }

    // GET /api/cinemas/1
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CinemaDto>> GetCinema(int id)
    {
        var cinema = await _context.Cinemas
            .AsNoTracking()
            .Where(c => c.CinemaId == id)
            .Select(ToDto)
            .FirstOrDefaultAsync();

        if (cinema == null) return CinemaNotFound(id);

        return Ok(cinema);
    }

    // GET /api/cinemas/1/screenings  -> alle screenings i en biograf
    [HttpGet("{cinemaId:int}/screenings")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ScreeningDto>>> GetCinemaScreenings(int cinemaId)
    {
        if (!await _context.Cinemas.AnyAsync(c => c.CinemaId == cinemaId))
            return CinemaNotFound(cinemaId);

        var screenings = await _context.Screenings
            .AsNoTracking()
            .Where(s => s.Screen.CinemaId == cinemaId)
            .OrderBy(s => s.ScreeningDate).ThenBy(s => s.StartTime)
            .Select(ScreeningProjections.ToDto)
            .ToListAsync();

        return Ok(screenings);
    }

    private ActionResult CinemaNotFound(int id) =>
        Problem(detail: $"Cinema with id {id} was not found.", statusCode: StatusCodes.Status404NotFound);
}
