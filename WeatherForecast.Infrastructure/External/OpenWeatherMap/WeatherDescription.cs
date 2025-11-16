using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class WeatherDescription
{
    [JsonPropertyName("main")] public string Main { get; set; } = string.Empty;
    
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("icon")] public string Icon { get; set; } = string.Empty;
}