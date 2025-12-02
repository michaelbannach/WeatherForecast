using WeatherForecast.Domain.Models;
using WeatherForecast.Application.Dtos;


namespace WeatherForecast.Application.Mapping;

public static class EntityToDtoMapper
{
    public static FavoriteDto ToDto(Favorite entity) =>
        new FavoriteDto(entity.Id, entity.City, entity.Country);
    
}