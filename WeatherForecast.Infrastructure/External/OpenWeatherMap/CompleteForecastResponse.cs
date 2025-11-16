using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class CompleteForecastResponse
{
    [JsonPropertyName("list")]
    public List<ForecastListEntry> List { get; set; }

    [JsonPropertyName("city")]
    public ForecastCity City { get; set; }
}