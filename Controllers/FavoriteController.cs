using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetFavoritesAsync(string userId)
    {
        try
        {
            var data = await _favoriteService.GetFavoritesAsync(userId);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> AddFavoriteAsync(string userId, Favorite favorite)
    {
        var (added, error) = await _favoriteService.AddFavoriteAsync(userId, favorite);
        if (!added)
        {
            return BadRequest(new { message = error });
        }

        return Ok();
    }
 
    [HttpDelete("{userId}/{id}")]
    public async Task<IActionResult> DeleteFavoriteAsync(string userId, int id)
    {
        var deleted = await _favoriteService.DeleteByIdAsync(userId, id);
        if (!deleted)
            return NotFound(new { message = "Favorit nicht gefunden oder kein Zugriff." });

        return NoContent();
    }

}


