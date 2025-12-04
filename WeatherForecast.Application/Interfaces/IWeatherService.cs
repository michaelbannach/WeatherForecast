using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces;

public interface IWeatherService
{
    Task<(Weather? data, string? error)> GetWeatherAsync(string city, string country);
    
    Task<(List<Forecast>? data, string? error)> GetThreeDayForecastAsync(string city, string country);

    Task<(List<Forecast>? data, string? error)> GetFiveDayForecastAsync(string city, string country);
}