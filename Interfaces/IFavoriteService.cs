using WeatherForecast.Models;


namespace WeatherForecast.Interfaces;

public interface IFavoriteService
{
    Task<List<Favorite>> GetFavoritesAsync(string userId);
    Task <bool> AddAsync(Favorite favorite);
    
    Task<bool>DeleteByIdAsync(string userId, int id);
}