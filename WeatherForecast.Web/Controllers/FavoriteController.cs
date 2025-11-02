using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFavoritesAsync()
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
            return Unauthorized();
        
        try
        {
            var favorites = await _favoriteService.GetFavoritesAsync(userId);
            return Ok(favorites);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> AddFavoriteAsync([FromBody] Favorite favorite)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
            return Unauthorized();
        try
        {
            await _favoriteService.AddFavoriteAsync(userId, favorite);
            return Ok();
        }

        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
       
    }
 
    [HttpDelete("{favoriteId}")]
    public async Task<IActionResult> DeleteByIdAsync(int favoriteId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
            return Unauthorized();

        try
        {
            await _favoriteService.DeleteByIdAsync(userId, favoriteId);
            return Ok();

        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
       
    }

}


