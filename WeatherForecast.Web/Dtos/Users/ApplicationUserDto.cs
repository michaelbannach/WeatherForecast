using WeatherForecast.Web.Dtos.Favorites;

namespace WeatherForecast.Web.Dtos.Users;



public record ApplicationUserDto( 
    string UserId,
    string FirstName,
    string LastName,
    List<FavoriteDto> Favorites
    );

    
