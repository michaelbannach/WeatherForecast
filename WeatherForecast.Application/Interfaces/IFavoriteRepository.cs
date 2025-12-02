using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces;

public interface IFavoriteRepository
{
    Task<List<Favorite>> GetFavoritesAsync(Guid userId);
    Task<bool> AddFavoriteAsync(Favorite favorite);
    Task<bool> DeleteByIdAsync(Guid userId, int id);
    Task<int> CountFavoritesAsync(Guid userId);
    Task<bool> AlreadyExistsAsync(Guid userId, string city, string country);
}
