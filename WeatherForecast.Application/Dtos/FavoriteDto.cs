namespace WeatherForecast.Application.Dtos;

public record FavoriteDto(
    int Id,
    string City,
    string Country
);