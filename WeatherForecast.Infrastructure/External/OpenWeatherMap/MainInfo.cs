using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class MainInfo
{
    [JsonPropertyName("temp")] public double Temp { get; set; }
    
    [JsonPropertyName("feels_like")] public double FeelsLike { get; set; }
    
    [JsonPropertyName("temp_min")] public double TempMin { get; set; }
    
    [JsonPropertyName("temp_max")] public double TempMax { get; set; }
    
    [JsonPropertyName("humidity")] public double Humidity { get; set; }
}