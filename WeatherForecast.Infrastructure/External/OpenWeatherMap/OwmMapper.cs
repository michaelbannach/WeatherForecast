using WeatherForecast.Application.Dtos;

namespace WeatherForecast.Infrastructure.External.OpenWeatherMap;

public static class OwmMapper
{
    public static WeatherDto ToAppDto(this CompleteWeatherResponse owm, string fallbackCity, string country)
    {
        var w = (owm.Weather != null && owm.Weather.Count > 0) ? owm.Weather[0] : null;
        return new WeatherDto
        {
            City       = owm.Name ?? fallbackCity,
            Country    = country,
            Temp       = owm.Main?.Temp        ?? 0,
            FeelsLike  = owm.Main?.FeelsLike   ?? 0,
            TempMin    = owm.Main?.TempMin     ?? 0,
            TempMax    = owm.Main?.TempMax     ?? 0,
            Humidity   = owm.Main?.Humidity    ?? 0,
            WindSpeed  = owm.Wind?.Speed       ?? 0,
            Sunrise    = owm.Sys?.Sunrise      ?? 0,
            Sunset     = owm.Sys?.Sunset       ?? 0,
            Summary    = w?.Main        ?? string.Empty,
            Description= w?.Description ?? string.Empty,
            Icon       = w?.Icon        ?? string.Empty
        };
    }

    public static List<ForecastDto> ToForecastDtos(this CompleteForecastResponse forecastResponse)
    {
        var groupByDate = forecastResponse.List
            .GroupBy(e => DateTime.Parse(e.DtTxt).Date)
            .Select(g =>
            {
                var minTemp = g.Min(x => x.Main.TempMin);
                var maxTemp = g.Max(x => x.Main.TempMax);
                
                var firstWeather = g.FirstOrDefault()?.Weather?.FirstOrDefault();

                return new ForecastDto
                {
                    Date = g.Key,
                    TempMin = minTemp,
                    TempMax = maxTemp,
                    Description = firstWeather?.Description ?? string.Empty,
                    Icon = firstWeather?.Icon ?? string.Empty
                };
            })
            .ToList();
        
        return groupByDate;
    }
}