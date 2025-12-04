using WeatherForecast.Web.Weather.Dtos;

namespace WeatherForecast.Web.Mappings;

public static class WeatherMappings
{
    public static WeatherDto ToDto(this WeatherForecast.Domain.Models.Weather weather)
        => new()
        {
            City = weather.City,
            Country = weather.Country,
            Temp = weather.Temp,
            FeelsLike = weather.FeelsLike,
            TempMin = weather.TempMin,
            TempMax = weather.TempMax,
            Humidity = weather.Humidity,
            WindSpeed = weather.WindSpeed,
            Sunrise = weather.Sunrise,
            Sunset = weather.Sunset,
            Summary = weather.Summary,
            Description = weather.Description,
            Icon = weather.Icon
        };

    public static ForecastDto ToDto(this WeatherForecast.Domain.Models.Forecast forecast)
        => new()
        {
            Date = forecast.Date,
            TempMin = forecast.TempMin,
            TempMax = forecast.TempMax,
            Description = forecast.Description,
            Icon = forecast.Icon
        };
}