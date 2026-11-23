using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/auth")]

public class AccountController : ControllerBase
{
private readonly MovieVaultContext _context;
private readonly ILogger<AccountController> _logger; //"DomainsController" SPØRG FOR SATAN, SÅ SPØRG DOG. JEG SKAL I TEATRET MED MIN KONE
private readonly IConfiguration _configuration;
private readonly UserManager<ApiUser> _userManager;
private readonly SignInManager<ApiUser> _signInManager;
public AccountController(
        MovieVaultContext context,
        ILogger<AccountController> logger, //"DomainsController" SPØRG FOR SATAN, SÅ SPØRG DOG.
        IConfiguration configuration,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }


[HttpPost("register")]
public async Task<ActionResult> Register(RegisterDTO input)
{
    try
    {
        if (ModelState.IsValid)
        {
            var newUser = new ApiUser();
            newUser.UserName = input.Email;
            newUser.Email = input.Email;
            newUser.FullName = input.FullName;
            var result = await _userManager.CreateAsync(
            newUser, input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation(
                "User {userName} ({email}) has been created.",
                newUser.UserName, newUser.Email);
                return StatusCode(201,
                $"User '{newUser.UserName}' has been created.");
            }
            else
                throw new Exception(
                string.Format("Error: {0}", string.Join(" ",
                result.Errors.Select(e => e.Description))));
         }
                 else
            return BadRequest(new ValidationProblemDetails(ModelState));   // ugyldigt input: 400
    }
    catch (Exception e)
    {
        return StatusCode(500, new ProblemDetails { Detail = e.Message, Status = 500 });
    }
}


[HttpPost("login")]
public async Task<ActionResult> Login(LoginDTO input)
{
    try
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, input.Password))
                throw new Exception("Invalid login attempt.");
            else
            {
                var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"])),
                    SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                var jwtObject = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddSeconds(300),
                    signingCredentials: signingCredentials);
                var jwtString = new JwtSecurityTokenHandler()
                .WriteToken(jwtObject);
                return StatusCode(StatusCodes.Status200OK, jwtString);
            }       }
                else
        {
            return BadRequest(new ValidationProblemDetails(ModelState));   // 400
        }
    }
    catch (Exception e)
    {
        return StatusCode(StatusCodes.Status401Unauthorized,
            new ProblemDetails { Detail = e.Message, Status = 401 });
    }
}


}
