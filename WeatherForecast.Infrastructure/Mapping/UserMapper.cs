using WeatherForecast.Application.Dtos;
using WeatherForecast.Application.Mapping;
using WeatherForecast.Infrastructure.Models;


namespace WeatherForecast.Infrastructure.Mapping;

public static class UserMapper
{
    public static ApplicationUserDto ToDto(this ApplicationUser user)

    {
         return new ApplicationUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Favorites
                .Select(f => EntityToDtoMapper.ToDto(f))
                .ToList()
        );
    }
}