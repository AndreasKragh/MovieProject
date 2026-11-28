using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Authorization;
using MovieVault.Api.Constants;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Controllers;

// Default-block, opt-allow (slide 35): alt kræver login, undtagen læsning, som er åben.
// Læsning af anmeldelser følger IKKE film-reglerne: alle kan se alle anmeldelser.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly MovieVaultContext _context;
    private readonly IAuthorizationService _authorizationService;

    public ReviewsController(MovieVaultContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;   // ejerskabstjek med ReviewOwnerHandler
    }

    // Én fælles beskrivelse af Review -> DTO. EF laver JOINs til Movie, Reviewer/AspNetUsers og MediaOutlet.
    private static readonly Expression<Func<Review, ReviewDto>> ToDto = r => new ReviewDto(
        r.ReviewId,
        r.Headline,
        r.Body,
        r.Rating,
        r.PublishedAt,
        r.MovieId,
        r.Movie.Title,
        r.ReviewerId,
        r.Reviewer.ApiUser.FullName ?? r.Reviewer.ApiUser.UserName ?? "",
        r.MediaOutletId,
        r.MediaOutlet.Name,
        r.MediaOutlet.OutletType);

    // ------------------------------------------------------------------
    // GET /api/reviews  (evt. ?movieId=1 og/eller ?mediaOutletId=2)
    // ------------------------------------------------------------------
    [AllowAnonymous]
    [HttpGet]
    [EndpointSummary("Get reviews (public)")]
    [EndpointDescription("Returns all reviews, newest first. Filter by movie with ?movieId=1 and by media outlet with ?mediaOutletId=2. The filters can be combined; omitted filters are ignored. No authentication required.")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews(
        [FromQuery] int? movieId, [FromQuery] int? mediaOutletId)
    {
        var query = _context.Reviews.AsNoTracking();

        if (movieId != null)
            query = query.Where(r => r.MovieId == movieId);
        if (mediaOutletId != null)
            query = query.Where(r => r.MediaOutletId == mediaOutletId);

        var reviews = await query
            .OrderByDescending(r => r.PublishedAt)
            .Select(ToDto)
            .ToListAsync();

        return Ok(reviews);
    }

    // ------------------------------------------------------------------
    // GET /api/reviews/3
    // ------------------------------------------------------------------
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [EndpointSummary("Get a review by id (public)")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        var review = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewId == id)
            .Select(ToDto)
            .FirstOrDefaultAsync();

        if (review == null) return ReviewNotFound(id);

        return Ok(review);
    }

    // ------------------------------------------------------------------
    // POST /api/reviews  -> opret anmeldelse
    // ------------------------------------------------------------------
    [Authorize(Policy = Policies.ReviewerOrAdmin)]   // andre roller: 403
    [HttpPost]
    [EndpointSummary("Create a review (Reviewer or Admin)")]
    [EndpointDescription("A Reviewer always writes as themselves: the author is taken from the token and the media outlet from the reviewer's profile. Only an Admin may set ReviewerId to create a review for a specific reviewer.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto dto)
    {
        // Forfatteren bestemmes ud fra tokenet (NameIdentifier), aldrig ud fra klientens ønske
        var ownReviewer = await _context.Reviewers
            .FirstOrDefaultAsync(r => r.ApiUserId == User.GetUserId());

        Reviewer? reviewer;
        if (dto.ReviewerId == null || dto.ReviewerId == ownReviewer?.ReviewerId)
            reviewer = ownReviewer;
        else if (User.IsAdmin())
            reviewer = await _context.Reviewers.FindAsync(dto.ReviewerId.Value);
        else
            return Forbid();   // 403: en Reviewer forsøger at skrive på vegne af en anden

        if (reviewer == null)
        {
            ModelState.AddModelError(nameof(dto.ReviewerId), dto.ReviewerId == null
                ? "No reviewer profile is linked to your account. An Admin must specify ReviewerId."
                : $"Reviewer with id {dto.ReviewerId} does not exist.");
        }
        if (!await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId))
            ModelState.AddModelError(nameof(dto.MovieId), $"Movie with id {dto.MovieId} does not exist.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);   // 400

        var review = new Review
        {
            MovieId = dto.MovieId,
            Headline = dto.Headline,
            Body = dto.Body,
            Rating = dto.Rating,
            ReviewerId = reviewer!.ReviewerId,
            MediaOutletId = reviewer.MediaOutletId,   // det medie, anmelderen repræsenterer
            PublishedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var created = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewId == review.ReviewId)
            .Select(ToDto)
            .FirstAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, created);
    }

    // ------------------------------------------------------------------
    // PUT /api/reviews/3  -> ret egen anmeldelse (Admin: alle)
    // ------------------------------------------------------------------
    [Authorize(Policy = Policies.ReviewerOrAdmin)]
    [HttpPut("{id:int}")]
    [EndpointSummary("Update a review (own reviews, or any as Admin)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto dto)
    {
        var review = await _context.Reviews
            .Include(r => r.Reviewer)   // handleren skal bruge Reviewer.ApiUserId
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (review == null) return ReviewNotFound(id);

        // Resource-based authorization (slide 60): er det brugerens egen anmeldelse?
        var authResult = await _authorizationService.AuthorizeAsync(User, review, Policies.CanManageReview);
        if (!authResult.Succeeded) return Forbid();   // 403: en andens anmeldelse

        review.Headline = dto.Headline;
        review.Body = dto.Body;
        review.Rating = dto.Rating;

        await _context.SaveChangesAsync();
        return NoContent(); // 204
    }

    // ------------------------------------------------------------------
    // DELETE /api/reviews/3  -> slet egen anmeldelse (Admin: alle)
    // ------------------------------------------------------------------
    [Authorize(Policy = Policies.ReviewerOrAdmin)]
    [HttpDelete("{id:int}")]
    [EndpointSummary("Delete a review (own reviews, or any as Admin)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Reviewer)
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (review == null) return ReviewNotFound(id);

        var authResult = await _authorizationService.AuthorizeAsync(User, review, Policies.CanManageReview);
        if (!authResult.Succeeded) return Forbid();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }

    private ActionResult ReviewNotFound(int id) =>
        Problem(detail: $"Review with id {id} was not found.", statusCode: StatusCodes.Status404NotFound);
}
