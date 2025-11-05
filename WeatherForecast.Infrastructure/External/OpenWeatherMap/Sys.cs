using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class Sys
{
    [JsonPropertyName("sunrise")] public double Sunrise { get; set; }
    
    [JsonPropertyName("sunset")] public double Sunset { get; set; }
}