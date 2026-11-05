using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Dtos;

namespace MovieVault.Api.Controllers;

// Query-endpoint 4. Del af MoviesController (partial class).
public partial class MoviesController
{
    // GET /api/movies/ticket-sales
    // Pr. film: samlet antal solgte billetter og samlet omsætning.
    // Film uden screenings eller salg kommer med og får 0.
    [HttpGet("ticket-sales")]
    public async Task<ActionResult<IEnumerable<MovieTicketSalesDto>>> GetTicketSalesByMovie()
    {
        var result = await _context.Movies
            .AsNoTracking()
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
