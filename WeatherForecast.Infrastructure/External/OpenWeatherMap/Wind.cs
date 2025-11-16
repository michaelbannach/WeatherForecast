using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class Wind
{
    [JsonPropertyName("speed")] public double Speed { get; set; }
}