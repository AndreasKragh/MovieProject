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
        await CreateUserAsync(userManager, "reviewer1@movievault.dk", "Demo Reviewer 1", demoPassword, RoleNames.Reviewer);
        await CreateUserAsync(userManager, "reviewer2@movievault.dk", "Demo Reviewer 2", demoPassword, RoleNames.Reviewer);

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
    }

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
