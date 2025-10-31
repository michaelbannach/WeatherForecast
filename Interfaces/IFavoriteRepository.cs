using WeatherForecast.Models;


namespace WeatherForecast.Interfaces;

public interface IFavoriteRepository
{
    Task<List<Favorite>> GetFavoritesAsync(string userId);
    Task <bool> AddFavoriteAsync(Favorite favorite);
    
    Task<bool>DeleteByIdAsync(string userId, int id);
    
    Task<int>CountFavoritesAsync(string userId);

    Task<bool> AllreadyExistAsync(string userId, string city, string country);
}