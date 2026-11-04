using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketSalesController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public TicketSalesController(MovieVaultContext context)
    {
        _context = context;
    }

    // Én fælles beskrivelse af, hvordan en TicketSale omsættes til en DTO.
    // Bruges i alle læse-endpoints, og EF omsætter den til SQL med JOINs
    // (TicketSales -> Screenings -> Movies / Screens -> Cinemas).
    private static readonly Expression<Func<TicketSale, TicketSaleDto>> ToDto = t => new TicketSaleDto(
        t.TicketSaleId,
        t.ScreeningId,
        t.Screening.Movie.Title,
        t.Screening.Screen.Cinema.Name,
        t.Screening.Screen.ScreenNumber,
        t.Screening.ScreeningDate,
        t.Screening.StartTime,
        t.SaleDateTime,
        t.Quantity,
        t.TotalPrice);

    // ------------------------------------------------------------------
    // GET /api/ticketsales            (evt. ?screeningId=5)
    // ------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketSaleDto>>> GetTicketSales([FromQuery] int? screeningId)
    {
        var query = _context.TicketSales.AsNoTracking();

        if (screeningId != null)
            query = query.Where(t => t.ScreeningId == screeningId);

        var sales = await query
            .OrderBy(t => t.TicketSaleId)
            .Select(ToDto)
            .ToListAsync();

        return Ok(sales);
    }

    // ------------------------------------------------------------------
    // GET /api/ticketsales/3
    // ------------------------------------------------------------------
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketSaleDto>> GetTicketSale(int id)
    {
        var sale = await _context.TicketSales
            .AsNoTracking()
            .Where(t => t.TicketSaleId == id)
            .Select(ToDto)
            .FirstOrDefaultAsync();

        if (sale == null) return SaleNotFound(id);

        return Ok(sale);
    }

    // ------------------------------------------------------------------
    // POST /api/ticketsales  -> registrer et billetsalg
    // ------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketSaleDto>> CreateTicketSale(CreateTicketSaleDto dto)
    {
        // Quantity >= 1 er allerede valideret af attributten på DTO'en.

        // 1) Findes screeningen? (vi skal bruge salen og prisen)
        var screening = await _context.Screenings
            .Include(s => s.Screen)
            .FirstOrDefaultAsync(s => s.ScreeningId == dto.ScreeningId);

        if (screening == null)
        {
            ModelState.AddModelError(nameof(dto.ScreeningId),
                $"Screening with id {dto.ScreeningId} does not exist.");
            return ValidationProblem(ModelState);
        }

        // 2) Er der nok ledige pladser?
        var seatsAvailable = await GetSeatsAvailableAsync(screening);
        if (dto.Quantity > seatsAvailable)
            return CapacityExceeded(dto.Quantity, seatsAvailable);

        // 3) Opret salget. Serveren beregner prisen (klienten kan ikke selv sætte den).
        var sale = new TicketSale
        {
            ScreeningId = screening.ScreeningId,
            Quantity = dto.Quantity,
            TotalPrice = screening.Price * dto.Quantity,
            SaleDateTime = DateTime.UtcNow
        };

        _context.TicketSales.Add(sale);
        await _context.SaveChangesAsync();

        var created = await _context.TicketSales
            .AsNoTracking()
            .Where(t => t.TicketSaleId == sale.TicketSaleId)
            .Select(ToDto)
            .FirstAsync();

        return CreatedAtAction(nameof(GetTicketSale), new { id = sale.TicketSaleId }, created);
    }

    // ------------------------------------------------------------------
    // PUT /api/ticketsales/3  -> ret antallet
    // ------------------------------------------------------------------
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTicketSale(int id, UpdateTicketSaleDto dto)
    {
        var sale = await _context.TicketSales
            .Include(t => t.Screening).ThenInclude(s => s.Screen)
            .FirstOrDefaultAsync(t => t.TicketSaleId == id);

        if (sale == null) return SaleNotFound(id);

        // Kapacitetstjek igen. Salget selv tæller ikke med (ellers ville det blokere sig selv).
        var seatsAvailable = await GetSeatsAvailableAsync(sale.Screening, excludeSaleId: id);
        if (dto.Quantity > seatsAvailable)
            return CapacityExceeded(dto.Quantity, seatsAvailable);

        sale.Quantity = dto.Quantity;
        sale.TotalPrice = sale.Screening.Price * dto.Quantity;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ------------------------------------------------------------------
    // DELETE /api/ticketsales/3
    // ------------------------------------------------------------------
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTicketSale(int id)
    {
        var sale = await _context.TicketSales.FindAsync(id);
        if (sale == null) return SaleNotFound(id);

        _context.TicketSales.Remove(sale);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==================================================================
    // Hjælpemetoder
    // ==================================================================

    // Ledige pladser = salens kapacitet - billetter, der allerede er solgt til screeningen.
    // SQL: SELECT SUM(Quantity) FROM TicketSales WHERE ScreeningId = @id
    private async Task<int> GetSeatsAvailableAsync(Screening screening, int? excludeSaleId = null)
    {
        var alreadySold = await _context.TicketSales
            .Where(t => t.ScreeningId == screening.ScreeningId && t.TicketSaleId != excludeSaleId)
            .SumAsync(t => t.Quantity);

        return screening.Screen.SeatingCapacity - alreadySold;
    }

    private ActionResult CapacityExceeded(int requested, int available) =>
        Problem(
            detail: $"Not enough seats. Requested {requested}, but only {Math.Max(available, 0)} seat(s) are available for this screening.",
            statusCode: StatusCodes.Status409Conflict);

    private ActionResult SaleNotFound(int id) =>
        Problem(detail: $"Ticket sale with id {id} was not found.", statusCode: StatusCodes.Status404NotFound);
}
