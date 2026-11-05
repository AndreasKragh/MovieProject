using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class ScreeningsController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public ScreeningsController(MovieVaultContext context)
    {
        _context = context;
    }

    // GET /api/screenings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScreeningDto>>> GetScreenings()
    {
        var screenings = await _context.Screenings
            .AsNoTracking()
            .OrderBy(s => s.ScreeningDate).ThenBy(s => s.StartTime)
            .Select(ScreeningProjections.ToDto)
            .ToListAsync();

        return Ok(screenings);
    }

    // GET /api/screenings/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScreeningDto>> GetScreening(int id)
    {
        var screening = await _context.Screenings
            .AsNoTracking()
            .Where(s => s.ScreeningId == id)
            .Select(ScreeningProjections.ToDto)
            .FirstOrDefaultAsync();

        if (screening == null) return ScreeningNotFound(id);

        return Ok(screening);
    }

    // POST /api/screenings  -> opret screening for en film og knyt den til en sal
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ScreeningDto>> CreateScreening(ScreeningWriteDto dto)
    {
        var movie = await _context.Movies.FindAsync(dto.MovieId);
        var screen = await _context.Screens.FindAsync(dto.ScreenId);

        if (movie == null)
            ModelState.AddModelError(nameof(dto.MovieId), $"Movie with id {dto.MovieId} does not exist.");
        if (screen == null)
            ModelState.AddModelError(nameof(dto.ScreenId), $"Screen with id {dto.ScreenId} does not exist.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (await OverlapsAsync(dto.ScreenId, dto.ScreeningDate, dto.StartTime, movie!.Runtime, excludeId: null))
            return OverlapConflict();

        var screening = new Screening
        {
            MovieId = dto.MovieId,
            ScreenId = dto.ScreenId,
            ScreeningDate = dto.ScreeningDate,
            StartTime = dto.StartTime,
            Price = dto.Price
        };

        _context.Screenings.Add(screening);
        await _context.SaveChangesAsync();

        var created = await _context.Screenings
            .AsNoTracking()
            .Where(s => s.ScreeningId == screening.ScreeningId)
            .Select(ScreeningProjections.ToDto)
            .FirstAsync();

        return CreatedAtAction(nameof(GetScreening), new { id = screening.ScreeningId }, created);
    }

    // PUT /api/screenings/5  -> opdater screening
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateScreening(int id, ScreeningWriteDto dto)
    {
        var screening = await _context.Screenings.FindAsync(id);
        if (screening == null) return ScreeningNotFound(id);

        var movie = await _context.Movies.FindAsync(dto.MovieId);
        var screen = await _context.Screens.FindAsync(dto.ScreenId);

        if (movie == null)
            ModelState.AddModelError(nameof(dto.MovieId), $"Movie with id {dto.MovieId} does not exist.");
        if (screen == null)
            ModelState.AddModelError(nameof(dto.ScreenId), $"Screen with id {dto.ScreenId} does not exist.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var ticketsSold = await _context.TicketSales
            .Where(t => t.ScreeningId == id)
            .SumAsync(t => t.Quantity);

        // Billetsalg hører til en bestemt film, så filmen kan ikke skiftes efter salg
        if (ticketsSold > 0 && dto.MovieId != screening.MovieId)
        {
            return Problem(
                detail: "The movie cannot be changed because tickets have already been sold for this screening.",
                statusCode: StatusCodes.Status409Conflict);
        }

        // Må ikke flyttes til en sal, der er mindre end antallet af solgte billetter
        if (screen!.SeatingCapacity < ticketsSold)
        {
            return Problem(
                detail: $"The screen has {screen.SeatingCapacity} seats, but {ticketsSold} tickets are already sold for this screening.",
                statusCode: StatusCodes.Status409Conflict);
        }

        if (await OverlapsAsync(dto.ScreenId, dto.ScreeningDate, dto.StartTime, movie!.Runtime, excludeId: id))
            return OverlapConflict();

        screening.MovieId = dto.MovieId;
        screening.ScreenId = dto.ScreenId;
        screening.ScreeningDate = dto.ScreeningDate;
        screening.StartTime = dto.StartTime;
        screening.Price = dto.Price;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/screenings/5  -> annuller screening
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteScreening(int id)
    {
        var screening = await _context.Screenings.FindAsync(id);
        if (screening == null) return ScreeningNotFound(id);

        // TicketSales har DeleteBehavior.Restrict, så vi giver en forståelig fejl i stedet for en 500
        if (await _context.TicketSales.AnyAsync(t => t.ScreeningId == id))
        {
            return Problem(
                detail: "The screening cannot be cancelled because ticket sales exist for it.",
                statusCode: StatusCodes.Status409Conflict);
        }

        _context.Screenings.Remove(screening);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==================================================================
    // Hjælpemetoder
    // ==================================================================

    // To screenings i samme sal må ikke overlappe i tid (starttid + filmens spilletid).
    private async Task<bool> OverlapsAsync(int screenId, DateOnly date, TimeOnly time, int runtime, int? excludeId)
    {
        var start = date.ToDateTime(time);
        var end = start.AddMinutes(runtime);

        var from = date.AddDays(-1);
        var to = date.AddDays(1);

        var others = await _context.Screenings
            .AsNoTracking()
            .Where(s => s.ScreenId == screenId
                        && s.ScreeningId != excludeId
                        && s.ScreeningDate >= from
                        && s.ScreeningDate <= to)
            .Select(s => new { s.ScreeningDate, s.StartTime, s.Movie.Runtime })
            .ToListAsync();

        return others.Any(o =>
        {
            var otherStart = o.ScreeningDate.ToDateTime(o.StartTime);
            var otherEnd = otherStart.AddMinutes(o.Runtime);
            return start < otherEnd && otherStart < end;
        });
    }

    private ActionResult OverlapConflict() =>
        Problem(
            detail: "The screen is already booked for another screening at that time.",
            statusCode: StatusCodes.Status409Conflict);

    private ActionResult ScreeningNotFound(int id) =>
        Problem(detail: $"Screening with id {id} was not found.", statusCode: StatusCodes.Status404NotFound);
}

// Fælles omsætning Screening -> ScreeningDto, brugt af flere controllers.
// Er et Expression, så EF kan oversætte det til SQL (JOINs + SUM af solgte billetter).
public static class ScreeningProjections
{
    public static readonly Expression<Func<Screening, ScreeningDto>> ToDto = s => new ScreeningDto(
        s.ScreeningId,
        s.MovieId,
        s.Movie.Title,
        s.Screen.CinemaId,
        s.Screen.Cinema.Name,
        s.ScreenId,
        s.Screen.ScreenNumber,
        s.Screen.ScreenType,
        s.ScreeningDate,
        s.StartTime,
        s.Price,
        s.Screen.SeatingCapacity,
        s.TicketSales.Sum(t => t.Quantity),
        s.Screen.SeatingCapacity - s.TicketSales.Sum(t => t.Quantity));
}
