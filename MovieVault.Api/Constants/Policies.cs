namespace MovieVault.Api.Constants;

// Navnene på vores authorization policies samlet ét sted (slide 51-52).
// Selve reglerne (hvilke claims der kræves) defineres i Program.cs med AddAuthorization.
public static class Policies
{
    // Kun Admin: alle skrive-operationer (POST, PUT, DELETE) og senere GET /api/logs
    public const string AdminOnly = "AdminOnly";

    // "Ticket sales by movie": Actor, Director og Admin - Reviewer nægtes
    public const string ActorDirectorOrAdmin = "ActorDirectorOrAdmin";

    // "Actor movie performance": kun Actor og Admin
    public const string ActorOrAdmin = "ActorOrAdmin";
}
