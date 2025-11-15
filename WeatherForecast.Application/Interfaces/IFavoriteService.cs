using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherForecast.Application.Dtos;


namespace WeatherForecast.Application.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteDto>> GetFavoritesAsync(string userId);
    Task<(bool added, string? error)> AddFavoriteAsync(string userId, FavoriteDto favoriteDto);
    //Task<bool> DeleteByIdAsync(string userId, int id); -> Controller weiß nicht, warum es fehlschlug

    Task<(bool deleted, string? error)> DeleteByIdAsync(string userId, int id);
}