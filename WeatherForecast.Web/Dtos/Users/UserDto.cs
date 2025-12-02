namespace WeatherForecast.Web.Dtos.Users;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName
    );