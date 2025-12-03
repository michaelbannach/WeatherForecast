namespace WeatherForecast.Web.Dtos.Favorites;

public record FavoriteDto
{
    public int Id { get; init; }
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}