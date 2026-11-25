using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Constants;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Data;

// Seeder roller og demo-brugere ved opstart (slide 38-39 og 45).
// Kan køres hver gang appen starter: den opretter kun det, der mangler.
public static class UserSeeder
{
    public static async Task SeedUsersAsync(
        UserManager<ApiUser> userManager,
        RoleManager<IdentityRole> roleManager,
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
        await CreateUserAsync(userManager, "actor@movievault.dk",     "Demo Actor",     demoPassword, RoleNames.Actor);
        await CreateUserAsync(userManager, "director@movievault.dk",  "Demo Director",  demoPassword, RoleNames.Director);
        await CreateUserAsync(userManager, "reviewer1@movievault.dk", "Demo Reviewer 1", demoPassword, RoleNames.Reviewer);
        await CreateUserAsync(userManager, "reviewer2@movievault.dk", "Demo Reviewer 2", demoPassword, RoleNames.Reviewer);

        // 3) Separat Admin-konto til baggrundsservicen (ticket sale simulator).
        //    Brugernavn og adgangskode konfigureres særskilt fra demo-brugerne.
        var serviceUserName = configuration["ServiceAccount:UserName"]
            ?? throw new InvalidOperationException("ServiceAccount:UserName mangler i konfigurationen.");
        var servicePassword = configuration["ServiceAccount:Password"]
            ?? throw new InvalidOperationException("ServiceAccount:Password mangler i konfigurationen.");

        await CreateUserAsync(userManager, serviceUserName, "Ticket Sale Simulator", servicePassword, RoleNames.Admin);
    }

    private static async Task CreateUserAsync(
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
    }
}
