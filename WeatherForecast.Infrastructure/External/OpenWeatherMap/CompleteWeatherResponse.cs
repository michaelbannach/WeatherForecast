using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class CompleteWeatherResponse
{
   [JsonPropertyName("main")] public MainInfo Main { get; set; }
   [JsonPropertyName("weather")] public List<WeatherDescription> Weather { get; set; }
   [JsonPropertyName("wind")] public Wind Wind { get; set; }
   [JsonPropertyName("sys")] public Sys Sys { get; set; }
   [JsonPropertyName("name")] public string Name { get; set; }
    
    
}