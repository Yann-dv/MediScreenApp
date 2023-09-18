using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediScreenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MediScreenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    [Route("user")]
    public async Task<IActionResult> GetUserById([FromQuery] string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
    
    [HttpGet]
    [Route("userExists")]
    public async Task<IActionResult> UserExists([FromQuery] string userName, [FromQuery] string email)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user != null)
        {
            return Ok(true);
        }

        user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            return Ok(true);
        }

        return Ok(false);
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _signInManager.SignInAsync(user, false);

        return Ok();
    }
    
    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login(
        [FromQuery] string username,
        [FromQuery] string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return BadRequest("Invalid credentials");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded)
        {
            return BadRequest("Invalid credentials");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is the MediScreen Secret key for authentication)"));

        var token = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            expires: DateTime.UtcNow.AddHours(1),
            claims: claims,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
}
