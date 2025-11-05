
using WeatherForecast.Application.Dtos;

namespace WeatherForecast.Application.Interfaces;

public interface IWeatherService
{
    Task<(WeatherDto? data, string? error)> GetWeatherAsync(string city, string country);
}