using WeatherForecast.Models;


namespace WeatherForecast.Interfaces;

public interface IFavoriteService
{
    Task<List<Favorite>> GetFavoritesAsync(string userId);
    Task<(bool added, string? error)> AddFavoriteAsync(string userId, Favorite favorite);
    Task<bool> DeleteByIdAsync(string userId, int id);
}