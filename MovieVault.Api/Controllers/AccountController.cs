using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Dtos;
using MovieVault.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

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

}
