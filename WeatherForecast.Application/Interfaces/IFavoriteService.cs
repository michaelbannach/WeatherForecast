using WeatherForecast.Domain.Models;


namespace WeatherForecast.Application.Interfaces;

public interface IFavoriteService
{
    Task<List<Favorite>> GetFavoritesAsync(string applicationUserId);
    Task<(bool added, string? error)> AddFavoriteAsync(string applicationUserId, Favorite favorite);
   

    Task<(bool deleted, string? error)> DeleteByIdAsync(string applicationUserId, int id);
}