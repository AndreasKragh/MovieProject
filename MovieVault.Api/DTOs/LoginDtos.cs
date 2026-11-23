using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Dtos;

public class LoginDTO
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}