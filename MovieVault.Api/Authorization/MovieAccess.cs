using System.Security.Claims;
using MovieVault.Api.Constants;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Authorization;

// Fælles regler for "hvilke film må den indloggede bruger se?".
// Bruges både af MovieAccessHandler (én film -> 403) og af collection-endpoints (filtrering).
public static class MovieAccess
{
    // Brugerens Id (AspNetUsers.Id). Lægges i tokenet som NameIdentifier ved login.
    public static string? GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier);

    public static bool IsAdmin(this ClaimsPrincipal user) =>
        user.HasClaim(ClaimTypes.Role, RoleNames.Admin);

    // Admin og Reviewer ser alle film. Har en bruger flere roller, giver den
    // "bredeste" rolle adgangen, så Admin altid ser alt.
    public static bool CanSeeAllMovies(this ClaimsPrincipal user) =>
        user.IsAdmin() || user.HasClaim(ClaimTypes.Role, RoleNames.Reviewer);

    // "Own movies": film hvor brugeren er instruktør ELLER medvirker som skuespiller.
    // Kører som SQL (WHERE ... OR EXISTS ...), så kun de tilladte film hentes.
    public static IQueryable<Movie> OwnedBy(this IQueryable<Movie> movies, string? userId) =>
        movies.Where(m =>
            (m.Director != null && m.Director.ApiUserId == userId) ||
            m.Cast.Any(c => c.Actor.ApiUserId == userId));
}
