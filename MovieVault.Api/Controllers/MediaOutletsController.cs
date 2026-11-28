using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;

namespace MovieVault.Api.Controllers;

// Kun læsning: opgaven kræver ikke CRUD for medier (de seedes).
// Offentlig ligesom anmeldelserne, så man kan finde et medies id og filtrere
// anmeldelser med GET /api/reviews?mediaOutletId=...
[ApiController]
[Route("api/media-outlets")]
public class MediaOutletsController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public MediaOutletsController(MovieVaultContext context)
    {
        _context = context;
    }

    // GET /api/media-outlets
    [HttpGet]
    [EndpointSummary("Get media outlets (public)")]
    public async Task<ActionResult<IEnumerable<MediaOutletDto>>> GetMediaOutlets()
    {
        var outlets = await _context.MediaOutlets
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new MediaOutletDto(o.MediaOutletId, o.Name, o.OutletType, o.Website))
            .ToListAsync();

        return Ok(outlets);
    }

    // GET /api/media-outlets/1
    [HttpGet("{id:int}")]
    [EndpointSummary("Get a media outlet by id (public)")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaOutletDto>> GetMediaOutlet(int id)
    {
        var outlet = await _context.MediaOutlets
            .AsNoTracking()
            .Where(o => o.MediaOutletId == id)
            .Select(o => new MediaOutletDto(o.MediaOutletId, o.Name, o.OutletType, o.Website))
            .FirstOrDefaultAsync();

        if (outlet == null)
            return Problem(detail: $"Media outlet with id {id} was not found.", statusCode: StatusCodes.Status404NotFound);

        return Ok(outlet);
    }
}
