using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediScreenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Route("GetAllUsers")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
    {
        var users = new List<ApplicationUser>();

        if (_userManager.Users == null || !_userManager.Users.Any())
        {
            return NotFound();
        }

        return await _userManager.Users.ToListAsync();
    }
    
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        return BadRequest();
    }
    
    
    [HttpGet]
    [Route("UserExists")]
    public async Task<IActionResult> UserExists(string userName, string email)
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
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (!result.Succeeded)
        {
            return BadRequest("Invalid credentials");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey("this is the MediScreen Secret key for authentication)"u8.ToArray());

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