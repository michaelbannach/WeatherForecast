using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;  

namespace WeatherForecast.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserService _userService;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(
        IFavoriteRepository favoriteRepository, 
        IUserService userService, 
        ILogger<FavoriteService> logger)
    {
        _favoriteRepository = favoriteRepository;
        _userService = userService;
        _logger = logger;
    }

    private static string NormCity(string s) => s?.Trim() ?? string.Empty;
    private static string NormCountry(string s) => s?.Trim().ToUpper() ?? string.Empty;

    
    public async Task<List<Favorite>> GetFavoritesAsync(string applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new ArgumentException("UserId must not be empty");

        var domainUser = await _userService.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
            throw new ArgumentException("User not found");

        var favorites = await _favoriteRepository.GetFavoritesAsync(domainUser.Id);
        return favorites;  // ← Domain Models directly!
    }

    // Domain Model (not DTO!)
    public async Task<(bool added, string? error)> AddFavoriteAsync(
        string applicationUserId, 
        Favorite favorite)  // ← Domain Model!
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return (false, "Unknown user");

        var domainUser = await _userService.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
            return (false, "User not found");

        var city = NormCity(favorite.City);
        var country = NormCountry(favorite.Country);

        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
            return (false, "City and Country must not be empty");

        var exists = await _favoriteRepository.AlreadyExistsAsync(domainUser.Id, city, country);
        if (exists)
            return (false, "City already exists");

        var count = await _favoriteRepository.CountFavoritesAsync(domainUser.Id);
        if (count >= 5)
            return (false, "Maximal 5 allowed");

        var newFavorite = new Favorite
        {
            UserId = domainUser.Id,
            City = city,
            Country = country
        };

        var ok = await _favoriteRepository.AddFavoriteAsync(newFavorite);
        if (!ok)
            return (false, "Saving not possible");

        _logger.LogInformation("Favorite added: {City}/{Country} for User {UserId}", 
            city, country, domainUser.Id);
        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteByIdAsync(
        string applicationUserId, 
        int id)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return (false, "User must not be empty");

        var domainUser = await _userService.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
            return (false, "User not found");

        var deleted = await _favoriteRepository.DeleteByIdAsync(domainUser.Id, id);
        if (!deleted)
            return (false, "Favorite not found or deleted");

        _logger.LogInformation("Favorite deleted: ID {FavoriteId} from User {UserId}", id, domainUser.Id);
        return (true, null);
    }
}
