namespace MovieVault.Api.Entities;

// Anmelderprofil: kobler en Identity-bruger til det medie, personen arbejder for.
// Samme mønster som Actor/Director -> ApiUser. Navnet står ikke her, men hentes fra
// ApiUser.FullName, så det ikke gemmes to steder.
public class Reviewer
{
    public int ReviewerId { get; set; }

    // Kobling til Identity-brugeren (AspNetUsers.Id). Påkrævet og unik: én profil pr. bruger.
    public string ApiUserId { get; set; } = string.Empty;
    public ApiUser ApiUser { get; set; } = null!;

    // Det medie, anmelderen arbejder for nu
    public int MediaOutletId { get; set; }
    public MediaOutlet MediaOutlet { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
