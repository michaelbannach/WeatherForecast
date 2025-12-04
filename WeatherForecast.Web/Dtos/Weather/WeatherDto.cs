namespace WeatherForecast.Web.Weather.Dtos;

public sealed class WeatherDto
{
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;

    public double Temp { get; init; }
    public double FeelsLike { get; init; }
    public double TempMin { get; init; }
    public double TempMax { get; init; }
    public double Humidity { get; init; }
    public double WindSpeed { get; init; }

    public string Summary { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;

    public double Sunrise { get; init; }
    public double Sunset { get; init; }
}