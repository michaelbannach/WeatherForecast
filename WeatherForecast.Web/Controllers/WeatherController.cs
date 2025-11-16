using Microsoft.AspNetCore.Mvc;

using WeatherForecast.Application.Interfaces;

namespace WeatherForecast.Web.Controllers;

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
   
   [HttpGet("forecast/3days/{city}/{country}")]
   public async Task<IActionResult> GetThreeDayForecast(string city, string country)
   {
      var (data, error) = await _weatherService.GetThreeDayForecastAsync(city, country);

      if (error is not null)
         return BadRequest(error);

      if (data == null || data.Count == 0)
         return NotFound("Keine 3-Tage Vorhersagedaten gefunden.");

      return Ok(data);
   }

   [HttpGet("forecast/5days/{city}/{country}")]
   public async Task<IActionResult> GetFiveDayForecast(string city, string country)
   {
      var (data, error) = await _weatherService.GetFiveDayForecastAsync(city, country);

      if (error is not null)
         return BadRequest(error);

      if (data == null || data.Count == 0)
         return NotFound("Keine 5-Tage Vorhersagedaten gefunden.");

      return Ok(data);
   }
}