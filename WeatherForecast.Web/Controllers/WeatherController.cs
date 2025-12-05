using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Web.Mappings;
using WeatherForecast.Web.Weather.Dtos;

namespace WeatherForecast.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{city}/{country}")]
    public async Task<IActionResult> GetWeather(string city, string country)
    {
        var (weather, error) = await _weatherService.GetWeatherAsync(city, country);

        if (error is not null)
            return BadRequest(error);

        if (weather is null)
            return NotFound("No Weatherdata found.");

        WeatherDto dto = weather.ToDto();
        return Ok(dto);
    }

    [HttpGet("forecast/3days/{city}/{country}")]
    public async Task<IActionResult> GetThreeDayForecast(string city, string country)
    {
        var (forecasts, error) = await _weatherService.GetThreeDayForecastAsync(city, country);

        if (error is not null)
            return BadRequest(error);

        if (forecasts == null || forecasts.Count == 0)
            return NotFound("No 3-Days-Forecast-Data found.");

        var dtos = forecasts.Select(f => f.ToDto()).ToList();
        return Ok(dtos);
    }

    [HttpGet("forecast/5days/{city}/{country}")]
    public async Task<IActionResult> GetFiveDayForecast(string city, string country)
    {
        var (forecasts, error) = await _weatherService.GetFiveDayForecastAsync(city, country);

        if (error is not null)
            return BadRequest(error);

        if (forecasts == null || forecasts.Count == 0)
            return NotFound("No 5-Days-Forecast-Data found.");

        var dtos = forecasts.Select(f => f.ToDto()).ToList();
        return Ok(dtos);
    }
}
