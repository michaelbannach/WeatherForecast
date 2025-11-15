using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public class ForecastListEntry
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public MainInfo Main { get; set; }

    [JsonPropertyName("weather")]
    public List<WeatherDescription> Weather { get; set; }

    [JsonPropertyName("dt_txt")]
    public string DtTxt { get; set; }
}