namespace WeatherForecast.Web.Weather.Dtos;

public sealed class ForecastDto
{
    public DateTime Date { get; init; }
    public double TempMin { get; init; }
    public double TempMax { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
}