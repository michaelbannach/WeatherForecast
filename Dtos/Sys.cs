using System.Text.Json.Serialization;

namespace WeatherForecast.Dtos;

public class Sys
{
    [JsonPropertyName("sunrise")] public double Sunrise { get; set; }
    
    [JsonPropertyName("sunset")] public double Sunset { get; set; }
}