using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Dtos;

namespace MovieVault.Api.Controllers;

// Query-endpoint 3. Anden halvdel af ScreeningsController (partial class).
public partial class ScreeningsController
{
    // GET /api/screenings/upcoming
    // Kommende screenings sorteret kronologisk med filmtitel, biograf, sal, tidspunkt og pris.
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<UpcomingScreeningDto>>> GetUpcomingScreenings()
    {
        // "Nu" beregnes her og sendes som parametre til SQL
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var timeNow = TimeOnly.FromDateTime(now);

        var screenings = await _context.Screenings
            .AsNoTracking()
            // Filtrering: senere dato, eller i dag med en starttid, der ikke er passeret
            .Where(s => s.ScreeningDate > today ||
                        (s.ScreeningDate == today && s.StartTime >= timeNow))
            .OrderBy(s => s.ScreeningDate).ThenBy(s => s.StartTime)
            // JOINs: Screening -> Movie, Screen -> Cinema
            .Select(s => new UpcomingScreeningDto(
                s.ScreeningId,
                s.Movie.Title,
                s.Screen.Cinema.Name,
                s.Screen.ScreenNumber,
                s.ScreeningDate,
                s.StartTime,
                s.Price))
            .ToListAsync();

        return Ok(screenings);
    }
}
