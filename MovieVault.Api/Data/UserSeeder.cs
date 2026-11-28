using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Constants;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Data;

// Seeder roller og demo-brugere ved opstart (slide 38-39 og 45).
// Kan køres hver gang appen starter: den opretter kun det, der mangler.
public static class UserSeeder
{
    public static async Task SeedUsersAsync(
        UserManager<ApiUser> userManager,
        RoleManager<IdentityRole> roleManager,
        MovieVaultContext context,
        IConfiguration configuration)
    {
        // 1) Opret de fire roller, hvis de ikke findes
        foreach (var role in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 2) Demo-brugere til mennesker. Alle bruger samme demo-adgangskode fra konfigurationen.
        var demoPassword = configuration["SeedUsers:DemoPassword"]
            ?? throw new InvalidOperationException("SeedUsers:DemoPassword mangler i konfigurationen.");

        await CreateUserAsync(userManager, "admin@movievault.dk",     "Demo Admin",     demoPassword, RoleNames.Admin);
        var actorUser    = await CreateUserAsync(userManager, "actor@movievault.dk",    "Demo Actor",    demoPassword, RoleNames.Actor);
        var directorUser = await CreateUserAsync(userManager, "director@movievault.dk", "Demo Director", demoPassword, RoleNames.Director);
        var reviewer1User = await CreateUserAsync(userManager, "reviewer1@movievault.dk", "Demo Reviewer 1", demoPassword, RoleNames.Reviewer);
        var reviewer2User = await CreateUserAsync(userManager, "reviewer2@movievault.dk", "Demo Reviewer 2", demoPassword, RoleNames.Reviewer);

        // 3) Separat Admin-konto til baggrundsservicen (ticket sale simulator).
        //    Brugernavn og adgangskode konfigureres særskilt fra demo-brugerne.
        var serviceUserName = configuration["ServiceAccount:UserName"]
            ?? throw new InvalidOperationException("ServiceAccount:UserName mangler i konfigurationen.");
        var servicePassword = configuration["ServiceAccount:Password"]
            ?? throw new InvalidOperationException("ServiceAccount:Password mangler i konfigurationen.");

        await CreateUserAsync(userManager, serviceUserName, "Ticket Sale Simulator", servicePassword, RoleNames.Admin);

        // 4) Kobl demo-Actor og demo-Director til personer i SeedData, så "own movies" kan vises:
        //    Actor 1 (Jonas Beck) medvirker i film 1, 2 og 6.
        //    Director 1 (Anna Kristensen) har instrueret film 1 og 4.
        await LinkActorAsync(context, actorId: 1, actorUser.Id);
        await LinkDirectorAsync(context, directorId: 1, directorUser.Id);

        // 5) Anmelderprofiler: Reviewer 1 skriver for Morgenposten (avis), Reviewer 2 for Kanal Nord (tv)
        var reviewer1 = await EnsureReviewerAsync(context, reviewer1User.Id, mediaOutletId: 1);
        var reviewer2 = await EnsureReviewerAsync(context, reviewer2User.Id, mediaOutletId: 2);

        // 6) Demo-anmeldelser (kun hvis der ingen er). Viser: flere anmeldelser af samme film
        //    (film 1), anmeldelser fra forskellige medier, og to anmeldere med hver deres ejerskab.
        //    Film 5 og 6 har ingen anmeldelser.
        if (!await context.Reviews.AnyAsync())
        {
            context.Reviews.AddRange(
                NewReview(reviewer1, 1, 5, "A harbour noir that never lets go",
                    "Anna Kristensen builds tension scene by scene. The night photography of Nyhavn is stunning, and the final act lands perfectly.",
                    new DateTime(2024, 3, 16, 8, 0, 0, DateTimeKind.Utc)),
                NewReview(reviewer2, 1, 3, "Beautiful, but slow",
                    "Midnight Harbor looks fantastic, but the middle hour drifts. Worth seeing on a big screen for the visuals alone.",
                    new DateTime(2024, 3, 18, 19, 30, 0, DateTimeKind.Utc)),
                NewReview(reviewer1, 2, 4, "An orchard of quiet secrets",
                    "A patient family drama with two excellent lead performances. It asks a lot of its audience, and mostly earns it.",
                    new DateTime(2025, 6, 21, 7, 45, 0, DateTimeKind.Utc)),
                NewReview(reviewer2, 3, 4, "Cold landscapes, warm heart",
                    "Echoes of Tundra turns the Arctic into a character of its own. A gentle film that stays with you.",
                    new DateTime(2025, 11, 8, 20, 0, 0, DateTimeKind.Utc)),
                NewReview(reviewer1, 4, 2, "Style over substance",
                    "Neon Requiem is all glowing surfaces and very little underneath. The soundtrack is the best thing about it.",
                    new DateTime(2026, 10, 3, 9, 15, 0, DateTimeKind.Utc))
            );
            await context.SaveChangesAsync();
        }
    }

    private static async Task<Reviewer> EnsureReviewerAsync(MovieVaultContext context, string userId, int mediaOutletId)
    {
        var reviewer = await context.Reviewers.FirstOrDefaultAsync(r => r.ApiUserId == userId);
        if (reviewer == null)
        {
            reviewer = new Reviewer { ApiUserId = userId, MediaOutletId = mediaOutletId };
            context.Reviewers.Add(reviewer);
            await context.SaveChangesAsync();
        }
        return reviewer;
    }

    // Anmeldelsen får anmelderens nuværende medie, ligesom når en anmeldelse oprettes via API'et
    private static Review NewReview(Reviewer reviewer, int movieId, int rating, string headline, string body, DateTime publishedAt) =>
        new()
        {
            ReviewerId = reviewer.ReviewerId,
            MediaOutletId = reviewer.MediaOutletId,
            MovieId = movieId,
            Rating = rating,
            Headline = headline,
            Body = body,
            PublishedAt = publishedAt
        };

    private static async Task LinkActorAsync(MovieVaultContext context, int actorId, string userId)
    {
        var actor = await context.Actors.FirstOrDefaultAsync(a => a.ActorId == actorId);
        if (actor != null && actor.ApiUserId == null)
        {
            actor.ApiUserId = userId;
            await context.SaveChangesAsync();
        }
    }

    private static async Task LinkDirectorAsync(MovieVaultContext context, int directorId, string userId)
    {
        var director = await context.Directors.FirstOrDefaultAsync(d => d.DirectorId == directorId);
        if (director != null && director.ApiUserId == null)
        {
            director.ApiUserId = userId;
            await context.SaveChangesAsync();
        }
    }

    private static async Task<ApiUser> CreateUserAsync(
        UserManager<ApiUser> userManager,
        string email,
        string fullName,
        string password,
        string role)
    {
        var user = await userManager.FindByNameAsync(email);

        if (user == null)
        {
            user = new ApiUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            // CreateAsync hasher adgangskoden - den gemmes aldrig i klartekst (slide 28)
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Kunne ikke oprette {email}: " +
                    string.Join(" ", result.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);

        return user;
    }
}
