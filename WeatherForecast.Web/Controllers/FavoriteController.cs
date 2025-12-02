using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Dtos;

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
    [Authorize]//(Policy = "SuperUserOnly")// ]
    public async Task<IActionResult> AddFavoriteAsync([FromBody] FavoriteDto favoriteDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        try
        {
            var (added, error) = await _favoriteService.AddFavoriteAsync(userId, favoriteDto);

            if (!added)
            {
                // Spezieller Fall: keine Berechtigung
                if (error == "Keine Berechtigung zum Speichern")
                {
                    return Forbid(); // HTTP 403
                }

                // „normale“ Fehler (max. 5, Stadt existiert schon, etc.)
                return BadRequest(new { message = error ?? "Speichern fehlgeschlagen" });
            }

            return Ok(new { message = "Favorit gespeichert" });
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

        var (deleted,error) = await _favoriteService.DeleteByIdAsync(userId, favoriteId);

        if (!deleted)
        {
            return NotFound(new {message = error ?? "Favorit nicht gefunden." });
        }
        
        return Ok(new {message = "Favorit gelöscht"});
    }

}


