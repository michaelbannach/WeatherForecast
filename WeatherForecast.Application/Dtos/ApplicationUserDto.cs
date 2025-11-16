namespace WeatherForecast.Application.Dtos;

public record ApplicationUserDto(
    string UserId,
    string FirstName,
    string LastName,
    List<FavoriteDto> Favorites
);