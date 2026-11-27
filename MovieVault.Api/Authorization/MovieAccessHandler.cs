using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Authorization;

// Requirement (slide 57/62): en tom "markør"-klasse, der navngiver kravet.
public class CanViewMovieRequirement : IAuthorizationRequirement { }

// Handler (slide 58/63): resource-based - får den konkrete film med som "resource".
// Registreres som scoped, fordi den bruger DbContext (slide 59/64).
public class MovieAccessHandler : AuthorizationHandler<CanViewMovieRequirement, Movie>
{
    private readonly MovieVaultContext _context;

    public MovieAccessHandler(MovieVaultContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanViewMovieRequirement requirement,
        Movie resource)
    {
        // Admin og Reviewer må se enhver film
        if (context.User.CanSeeAllMovies())
        {
            context.Succeed(requirement);
            return;
        }

        // Actor/Director: kun hvis filmen er en af deres egne
        var userId = context.User.GetUserId();
        if (userId == null) return;

        var isOwnMovie = await _context.Movies
            .Where(m => m.MovieId == resource.MovieId)
            .OwnedBy(userId)
            .AnyAsync();

        if (isOwnMovie)
            context.Succeed(requirement);

        // Ellers gør vi ingenting -> requirement ikke opfyldt -> controlleren returnerer 403
    }
}
