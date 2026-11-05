using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;

namespace MovieVault.Api.Controllers;

// Skuespillere: kun læsning. Opgaven kræver ikke CRUD, men vi har brug for opslag
// (til at finde ActorId) og for query-endpoint 5.
[ApiController]
[Route("api/[controller]")]
public class ActorsController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public ActorsController(MovieVaultContext context)
    {
        _context = context;
    }

    // GET /api/actors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetActors()
    {
        var actors = await _context.Actors
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new ActorDto(a.ActorId, a.Name, a.BirthDate, a.Nationality, a.Gender))
            .ToListAsync();

        return Ok(actors);
    }

    // GET /api/actors/2/movie-performance   (query-endpoint 5)
    // De film skuespilleren medvirker i, og billetsalget for hver af dem + totaler.
    [HttpGet("{actorId:int}/movie-performance")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActorMoviePerformanceDto>> GetMoviePerformance(int actorId)
    {
        var actor = await _context.Actors
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ActorId == actorId);

        if (actor == null)
        {
            return Problem(
                detail: $"Actor with id {actorId} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        // Filtrering på MovieCast (join-tabellen), derefter JOIN til Movie og SUM af billetsalg
        var movies = await _context.MovieCasts
            .AsNoTracking()
            .Where(c => c.ActorId == actorId)
            .OrderBy(c => c.Movie.Title)
            .Select(c => new ActorMovieDto(
                c.MovieId,
                c.Movie.Title,
                c.CharacterName,
                c.RoleType,
                c.Movie.Screenings.SelectMany(s => s.TicketSales).Sum(t => t.Quantity)))
            .ToListAsync();

        var result = new ActorMoviePerformanceDto(
            actor.ActorId,
            actor.Name,
            movies.Sum(m => m.TicketsSold),
            movies);

        return Ok(result);
    }
}
