using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Data;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
   private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
    
}