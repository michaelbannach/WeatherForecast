using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Data;
using WeatherForecast.Models;

namespace WeatherForecast.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<List<Favorite>>> GetFavorites()
    {
        var user = await _userManager.GetUserAsync(User);
        if(user == null) return Unauthorized();

        var favorites = await _db.UserInputs
            .Where(x => x.UserId == user.Id)
            .OrderBy(x => x.Id)
            .Take(5)
            .ToListAsync();
        
        return Ok(favorites);
    }
    
}