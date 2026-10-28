using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Entities;

// Join-entitet for N:M mellem Movie og Actor (sammensat PK: MovieId + ActorId)
public class MovieCast
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public int ActorId { get; set; }
    public Actor Actor { get; set; } = null!;

    [MaxLength(100)]
    public string CharacterName { get; set; } = string.Empty;

    // 'lead', 'supporting' eller 'cameo' (check constraint i DbContext)
    [MaxLength(50)]
    public string RoleType { get; set; } = string.Empty;
}