using Microsoft.AspNetCore.Authorization;
using MovieVault.Api.Constants;
using MovieVault.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

// Query-endpoint 4. Del af MoviesController (partial class).
public partial class MoviesController
{
    // GET /api/movies/ticket-sales
    // Pr. film: samlet antal solgte billetter og samlet omsætning.
    // Film uden screenings eller salg kommer med og får 0.
    [Authorize(Policy = Policies.ActorDirectorOrAdmin)]
    [HttpGet("ticket-sales")]
    public async Task<ActionResult<IEnumerable<MovieTicketSalesDto>>> GetTicketSalesByMovie()
    {
        IQueryable<Movie> query = _context.Movies.AsNoTracking();

        // Policyen har allerede afvist Reviewer (403). Admin ser alle film,
        // Actor/Director kun salg for egne film.
        if (!User.IsAdmin())
            query = query.OwnedBy(User.GetUserId());

        var result = await query
            .OrderBy(m => m.Title)
            .Select(m => new MovieTicketSalesDto(
                m.MovieId,
                m.Title,
                // Movie -> Screenings -> TicketSales, derefter SUM
                m.Screenings.SelectMany(s => s.TicketSales).Sum(t => t.Quantity),
                m.Screenings.SelectMany(s => s.TicketSales).Sum(t => t.TotalPrice)))
            .ToListAsync();

        return Ok(result);
    }
}
