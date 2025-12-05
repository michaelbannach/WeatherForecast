using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Web.Dtos.Favorites;
using WeatherForecast.Web.Mappings;

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
        if (userId == null)
            return Unauthorized();

        try
        {
            
            var favorites = await _favoriteService.GetFavoritesAsync(userId);
            
            
            var dtos = favorites.Select(f => f.ToDto()).ToList();
            
            return Ok(dtos);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "SuperUserOnly")]
    public async Task<IActionResult> AddFavoriteAsync([FromBody] CreateFavoriteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        try
        {
            
            var favorite = dto.ToEntity();
            
            
            var (added, error) = await _favoriteService.AddFavoriteAsync(userId, favorite);
            
            if (!added)
            {
               
                if (error?.Contains("Authorization") == true)
                    return Forbid(); // HTTP 403
                
                
                return BadRequest(new { message = error ?? "Save failed" });
            }

            return Ok(new { message = "Favorite saved successfully" });
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
        if (userId == null)
            return Unauthorized();

        var (deleted, error) = await _favoriteService.DeleteByIdAsync(userId, favoriteId);
        
        if (!deleted)
            return NotFound(new { message = error ?? "Favorite not found" });

        return Ok(new { message = "Favorite deleted" });
    }
}
