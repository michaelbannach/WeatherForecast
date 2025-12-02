using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WeatherForecast.Application.Interfaces;
using WeatherForecast.Infrastructure.Models;
using WeatherForecast.Web.Dtos.Auth;

namespace WeatherForecast.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
   private readonly IAuthService _authService;
   
    public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
       
    {
      _authService = authService;
    }

    public record LoginDto(string Email, string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _authService.LoginAsync(dto.Email, dto.Password);
        if (!success)
            return Unauthorized(new { error });

        // später: JWT im Response zurückgeben
        return Ok(new { Message = "Login ok" });
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, error, appUserId, domainUserId) = await _authService.RegisterAsync(
            dto.Email, dto.Password, dto.FirstName, dto.LastName, dto.Role);

        if (!success)
            return BadRequest(new { error });

        return Ok(new 
        { 
            message = "User created successfully",
            appUserId,
            domainUserId 
        });
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok("Logged out");
    }
}