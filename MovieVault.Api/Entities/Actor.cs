using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

public class Actor
{
    public int ActorId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    [MaxLength(50)]
    public string Nationality { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    public ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();

    // Kobling til Identity-brugeren (AspNetUsers.Id), så serveren kan finde
    // "mine film" ud fra tokenet. Nullable: ikke alle skuespillere har et login.
    public string? ApiUserId { get; set; }
    public ApiUser? ApiUser { get; set; }
}