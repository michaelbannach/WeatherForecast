namespace WeatherForecast.Web.Dtos.Favorites;

public record CreateFavoriteDto
{
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}
