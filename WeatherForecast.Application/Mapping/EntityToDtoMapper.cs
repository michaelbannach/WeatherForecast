using WeatherForecast.Domain.Models;
using WeatherForecast.Application.Dtos;

namespace WeatherForecast.Application.Mapping;

public static class EntityToDtoMapper
{
    public static FavoriteDto ToDto(Favorite entity) =>
        new FavoriteDto(entity.Id, entity.City, entity.Country, entity.UserId);

    public static ApplicationUserDto ToDto(ApplicationUser user) =>
        new ApplicationUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Favorites.Select(ToDto).ToList()
        );
}