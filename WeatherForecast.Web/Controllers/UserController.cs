using Microsoft.AspNetCore.Authorization;
using WeatherForecast.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace WeatherForecast.Web.Controllers;

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