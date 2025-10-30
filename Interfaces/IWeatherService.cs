using System.Threading.Tasks;
using WeatherForecast.Dtos;

namespace WeatherForecast.Interfaces;

public interface IWeatherService
{
    Task<(CompleteWeatherResponse? data, string? error)> GetWeatherAsync(string city, string country);
}