using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

// Anden halvdel af MoviesController (partial class): operationer, der håndterer
// relationerne omkring en film. Bruger _context og MovieNotFound fra den anden fil.
public partial class MoviesController
{
    // ==================================================================
    // CAST (N:M Movie <-> Actor via MovieCast)
    // ==================================================================

    // GET /api/movies/6/cast
    [HttpGet("{movieId:int}/cast")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CastMemberDto>>> GetCast(int movieId)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == movieId))
            return MovieNotFound(movieId);

        var cast = await _context.MovieCasts
            .AsNoTracking()
            .Where(c => c.MovieId == movieId)
            .OrderBy(c => c.Actor.Name)
            .Select(c => new CastMemberDto(c.ActorId, c.Actor.Name, c.CharacterName, c.RoleType))
            .ToListAsync();

        return Ok(cast);
    }

    // POST /api/movies/6/cast  { "actorId": 4, "characterName": "...", "roleType": "lead" }
    [HttpPost("{movieId:int}/cast")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CastMemberDto>> AssignActor(int movieId, AssignActorDto dto)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == movieId))
            return MovieNotFound(movieId);

        var actor = await _context.Actors.FindAsync(dto.ActorId);
        if (actor == null)
        {
            ModelState.AddModelError(nameof(dto.ActorId), $"Actor with id {dto.ActorId} does not exist.");
            return ValidationProblem(ModelState);
        }

        // (MovieId, ActorId) er primærnøgle, så den samme skuespiller kan kun stå én gang pr. film
        if (await _context.MovieCasts.AnyAsync(c => c.MovieId == movieId && c.ActorId == dto.ActorId))
        {
            return Problem(
                detail: $"Actor {dto.ActorId} is already in the cast of movie {movieId}.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var cast = new MovieCast
        {
            MovieId = movieId,
            ActorId = dto.ActorId,
            CharacterName = dto.CharacterName,
            RoleType = dto.RoleType
        };

        _context.MovieCasts.Add(cast);
        await _context.SaveChangesAsync();

        var result = new CastMemberDto(actor.ActorId, actor.Name, cast.CharacterName, cast.RoleType);
        return CreatedAtAction(nameof(GetCast), new { movieId }, result);
    }

    // DELETE /api/movies/6/cast/4
    [HttpDelete("{movieId:int}/cast/{actorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveActor(int movieId, int actorId)
    {
        // FindAsync med sammensat nøgle: rækkefølgen er (MovieId, ActorId), som i HasKey
        var cast = await _context.MovieCasts.FindAsync(movieId, actorId);
        if (cast == null)
        {
            return Problem(
                detail: $"Actor {actorId} is not in the cast of movie {movieId}.",
                statusCode: StatusCodes.Status404NotFound);
        }

        _context.MovieCasts.Remove(cast);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==================================================================
    // FILMING LOCATIONS (N:M Movie <-> Location via FilmedAt)
    // ==================================================================

    // GET /api/movies/6/locations
    [HttpGet("{movieId:int}/locations")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FilmingLocationDto>>> GetFilmingLocations(int movieId)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == movieId))
            return MovieNotFound(movieId);

        var locations = await _context.FilmedAt
            .AsNoTracking()
            .Where(f => f.MovieId == movieId)
            .OrderBy(f => f.Location.Name)
            .Select(f => new FilmingLocationDto(
                f.LocationId, f.Location.Name, f.Location.City,
                f.Location.Country, f.Location.LocationType, f.NumberOfShootingDays))
            .ToListAsync();

        return Ok(locations);
    }

    // POST /api/movies/6/locations  { "locationId": 5, "numberOfShootingDays": 3 }
    [HttpPost("{movieId:int}/locations")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FilmingLocationDto>> AddFilmingLocation(int movieId, AddFilmingLocationDto dto)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == movieId))
            return MovieNotFound(movieId);

        var location = await _context.Locations.FindAsync(dto.LocationId);
        if (location == null)
        {
            ModelState.AddModelError(nameof(dto.LocationId), $"Location with id {dto.LocationId} does not exist.");
            return ValidationProblem(ModelState);
        }

        if (await _context.FilmedAt.AnyAsync(f => f.MovieId == movieId && f.LocationId == dto.LocationId))
        {
            return Problem(
                detail: $"Location {dto.LocationId} is already associated with movie {movieId}.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var filmedAt = new FilmedAt
        {
            MovieId = movieId,
            LocationId = dto.LocationId,
            NumberOfShootingDays = dto.NumberOfShootingDays
        };

        _context.FilmedAt.Add(filmedAt);
        await _context.SaveChangesAsync();

        var result = new FilmingLocationDto(
            location.LocationId, location.Name, location.City,
            location.Country, location.LocationType, filmedAt.NumberOfShootingDays);

        return CreatedAtAction(nameof(GetFilmingLocations), new { movieId }, result);
    }

    // DELETE /api/movies/6/locations/5
    [HttpDelete("{movieId:int}/locations/{locationId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFilmingLocation(int movieId, int locationId)
    {
        var filmedAt = await _context.FilmedAt.FindAsync(movieId, locationId);
        if (filmedAt == null)
        {
            return Problem(
                detail: $"Location {locationId} is not associated with movie {movieId}.",
                statusCode: StatusCodes.Status404NotFound);
        }

        _context.FilmedAt.Remove(filmedAt);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==================================================================
    // SCREENINGS FOR EN FILM
    // ==================================================================

    // GET /api/movies/4/screenings
    [HttpGet("{movieId:int}/screenings")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ScreeningDto>>> GetMovieScreenings(int movieId)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == movieId))
            return MovieNotFound(movieId);

        var screenings = await _context.Screenings
            .AsNoTracking()
            .Where(s => s.MovieId == movieId)
            .OrderBy(s => s.ScreeningDate).ThenBy(s => s.StartTime)
            .Select(ScreeningProjections.ToDto)
            .ToListAsync();

        return Ok(screenings);
    }
}
