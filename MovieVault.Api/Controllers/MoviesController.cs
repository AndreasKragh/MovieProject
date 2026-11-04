using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public MoviesController(MovieVaultContext context)
    {
        _context = context;
    }

    // ------------------------------------------------------------------
    // GET /api/movies  -> filmoversigt (query-endpoint 1)
    // ------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieOverviewDto>>> GetMovies()
    {
        var movies = await _context.Movies
            .AsNoTracking()
            .OrderBy(m => m.Title)
            .Select(m => new MovieOverviewDto(
                m.MovieId,
                m.Title,
                m.ReleaseDate,
                m.MovieStatus,
                m.Director != null ? m.Director.Name : null,
                m.ProductionCompany != null ? m.ProductionCompany.Name : null,
                m.ProductionRecord != null ? m.ProductionRecord.Budget : (decimal?)null))
            .ToListAsync();

        return Ok(movies);
    }

    // ------------------------------------------------------------------
    // GET /api/movies/3  -> detaljer for én film (query-endpoint 2)
    // ------------------------------------------------------------------
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailsDto>> GetMovie(int id)
    {
        var details = await GetDetailsAsync(id);
        if (details == null) return MovieNotFound(id);

        return Ok(details);
    }

    // ------------------------------------------------------------------
    // POST /api/movies  -> opret film
    // ------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieDetailsDto>> CreateMovie(MovieWriteDto dto)
    {
        // Attributterne på DTO'en er allerede valideret af [ApiController].
        // Her tjekker vi det, der kræver et opslag i databasen.
        if (!await ValidateReferencesAsync(dto))
            return ValidationProblem(ModelState);

        var movie = new Movie
        {
            Title = dto.Title,
            Runtime = dto.Runtime,
            ReleaseDate = dto.ReleaseDate,
            AgeRating = dto.AgeRating,
            MovieStatus = dto.MovieStatus,
            DirectorId = dto.DirectorId,
            ProductionCompanyId = dto.ProductionCompanyId!.Value
        };

        // Valgfri 1:1 – EF sætter selv MovieId på ProductionRecord ved SaveChanges
        if (dto.ProductionRecord != null)
        {
            movie.ProductionRecord = new ProductionRecord
            {
                Budget = dto.ProductionRecord.Budget,
                ProductionStartDate = dto.ProductionRecord.ProductionStartDate,
                ProductionEndDate = dto.ProductionRecord.ProductionEndDate
            };
        }

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var created = await GetDetailsAsync(movie.MovieId);

        // 201 Created + Location-header, der peger på GET /api/movies/{id}
        return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, created);
    }

    // ------------------------------------------------------------------
    // PUT /api/movies/3  -> erstat en films data
    // ------------------------------------------------------------------
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMovie(int id, MovieWriteDto dto)
    {
        var movie = await _context.Movies
            .Include(m => m.ProductionRecord)
            .FirstOrDefaultAsync(m => m.MovieId == id);

        if (movie == null) return MovieNotFound(id);

        if (!await ValidateReferencesAsync(dto))
            return ValidationProblem(ModelState);

        movie.Title = dto.Title;
        movie.Runtime = dto.Runtime;
        movie.ReleaseDate = dto.ReleaseDate;
        movie.AgeRating = dto.AgeRating;
        movie.MovieStatus = dto.MovieStatus;
        movie.DirectorId = dto.DirectorId;
        movie.ProductionCompanyId = dto.ProductionCompanyId!.Value;

        // PUT erstatter hele ressourcen, så produktionsdata følger requesten:
        if (dto.ProductionRecord == null)
        {
            // Ingen produktionsdata i requesten -> fjern evt. eksisterende
            if (movie.ProductionRecord != null)
                _context.ProductionRecords.Remove(movie.ProductionRecord);
        }
        else if (movie.ProductionRecord == null)
        {
            movie.ProductionRecord = new ProductionRecord
            {
                Budget = dto.ProductionRecord.Budget,
                ProductionStartDate = dto.ProductionRecord.ProductionStartDate,
                ProductionEndDate = dto.ProductionRecord.ProductionEndDate
            };
        }
        else
        {
            movie.ProductionRecord.Budget = dto.ProductionRecord.Budget;
            movie.ProductionRecord.ProductionStartDate = dto.ProductionRecord.ProductionStartDate;
            movie.ProductionRecord.ProductionEndDate = dto.ProductionRecord.ProductionEndDate;
        }

        await _context.SaveChangesAsync();
        return NoContent(); // 204
    }

    // ------------------------------------------------------------------
    // DELETE /api/movies/3  -> slet film
    // ------------------------------------------------------------------
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return MovieNotFound(id);

        // Screenings har DeleteBehavior.Restrict, så databasen ville afvise sletningen.
        // Vi tjekker selv og giver en forståelig fejl (409 Conflict) i stedet for en 500.
        if (await _context.Screenings.AnyAsync(s => s.MovieId == id))
        {
            return Problem(
                detail: "The movie cannot be deleted because it has screenings. Delete the screenings first.",
                statusCode: StatusCodes.Status409Conflict);
        }

        // Cast, FilmedAt og ProductionRecord slettes automatisk (Cascade i DbContext)
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }

    // ==================================================================
    // Hjælpemetoder
    // ==================================================================

    // Tjekker at de refererede rækker findes. Fejl lægges i ModelState og giver 400 med feltnavn.
    private async Task<bool> ValidateReferencesAsync(MovieWriteDto dto)
    {
        if (dto.DirectorId != null &&
            await _context.Directors.FindAsync(dto.DirectorId.Value) == null)
        {
            ModelState.AddModelError(nameof(dto.DirectorId),
                $"Director with id {dto.DirectorId} does not exist.");
        }

        if (dto.ProductionCompanyId == null)
        {
            ModelState.AddModelError(nameof(dto.ProductionCompanyId),
                "ProductionCompanyId is required.");
        }
        else if (await _context.ProductionCompanies.FindAsync(dto.ProductionCompanyId.Value) == null)
        {
            ModelState.AddModelError(nameof(dto.ProductionCompanyId),
                $"Production company with id {dto.ProductionCompanyId} does not exist.");
        }

        return ModelState.IsValid;
    }

    private ActionResult MovieNotFound(int id) =>
        Problem(detail: $"Movie with id {id} was not found.", statusCode: StatusCodes.Status404NotFound);

    private async Task<MovieDetailsDto?> GetDetailsAsync(int id)
    {
        var movie = await _context.Movies
            .AsNoTracking()
            .Include(m => m.Director)
            .Include(m => m.ProductionCompany)
            .Include(m => m.ProductionRecord)
            .Include(m => m.Cast).ThenInclude(c => c.Actor)
            .Include(m => m.FilmingLocations).ThenInclude(f => f.Location)
            .FirstOrDefaultAsync(m => m.MovieId == id);

        return movie == null ? null : ToDetailsDto(movie);
    }

    // Mapping: entity -> DTO
    private static MovieDetailsDto ToDetailsDto(Movie m) => new(
        m.MovieId,
        m.Title,
        m.Runtime,
        m.ReleaseDate,
        m.AgeRating,
        m.MovieStatus,
        m.Director == null
            ? null
            : new DirectorDto(m.Director.DirectorId, m.Director.Name, m.Director.BirthDate, m.Director.Nationality),
        m.ProductionCompany == null
            ? null
            : new ProductionCompanyDto(m.ProductionCompany.ProductionCompanyId, m.ProductionCompany.Name,
                                       m.ProductionCompany.FoundedYear, m.ProductionCompany.Country),
        m.ProductionRecord == null
            ? null
            : new ProductionRecordDto(m.ProductionRecord.Budget, m.ProductionRecord.ProductionStartDate,
                                      m.ProductionRecord.ProductionEndDate),
        m.Cast
            .Select(c => new CastMemberDto(c.ActorId, c.Actor.Name, c.CharacterName, c.RoleType))
            .ToList(),
        m.FilmingLocations
            .Select(f => new FilmingLocationDto(f.LocationId, f.Location.Name, f.Location.City,
                                                f.Location.Country, f.Location.LocationType, f.NumberOfShootingDays))
            .ToList());
}
