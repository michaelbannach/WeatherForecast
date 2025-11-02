using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Interfaces;
using WeatherForecast.Dtos;

namespace WeatherForecast.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
   private  readonly IWeatherService _weatherService;

   public WeatherController(IWeatherService weatherService)
   {
      _weatherService = weatherService;
   }
   [HttpGet("{city}/{country}")]
   public async Task<IActionResult> GetWeather(string city, string country)
   {
      var (data, error) = await _weatherService.GetWeatherAsync(city, country);

      if (error is not null)
         return BadRequest(error);

      if (data is null)
         return NotFound("Keine Wetterdaten gefunden.");

      return Ok(data);
   }
}