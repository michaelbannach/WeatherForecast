using System.Text.Json.Serialization;

namespace WeatherForecast.Dtos;

public class Wind
{
    [JsonPropertyName("speed")] public double Speed { get; set; }
}