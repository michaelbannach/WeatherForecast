using WeatherForecast.Domain.Models;
using WeatherForecast.Web.Dtos.Favorites;

namespace WeatherForecast.Web.Mappings;

public static class FavoriteMappings
{
    public static Favorite ToEntity(this CreateFavoriteDto dto)
    {
        return new Favorite
        {
            City = dto.City,
            Country = dto.Country
        };
    }

    public static FavoriteDto ToDto(this Favorite entity)
    {
        return new FavoriteDto
        {
            Id = entity.Id,
            City = entity.City,
            Country = entity.Country
        };
    }
}