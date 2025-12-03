using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;  // ← Domain Models nur!

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
            throw new ArgumentException("UserId darf nicht leer sein");

        var domainUser = await _userService.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
            throw new ArgumentException("Benutzer nicht gefunden");

        var favorites = await _favoriteRepository.GetFavoritesAsync(domainUser.Id);
        return favorites;  // ← Domain Models direkt!
    }

    // ✅ Nimmt Domain Model (nicht DTO!)
    public async Task<(bool added, string? error)> AddFavoriteAsync(
        string applicationUserId, 
        Favorite favorite)  // ← Domain Model!
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return (false, "Unbekannter Benutzer");

        var domainUser = await _userService.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
            return (false, "Benutzer nicht gefunden");

        var city = NormCity(favorite.City);
        var country = NormCountry(favorite.Country);

        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
            return (false, "Stadt und Land dürfen nicht leer sein");

        var exists = await _favoriteRepository.AlreadyExistsAsync(domainUser.Id, city, country);
        if (exists)
            return (false, "Diese Stadt ist bereits gespeichert");

        var count = await _favoriteRepository.CountFavoritesAsync(domainUser.Id);
        if (count >= 5)
            return (false, "Maximal 5 erlaubt");

        var newFavorite = new Favorite
        {
            UserId = domainUser.Id,
            City = city,
            Country = country
        };

        var ok = await _favoriteRepository.AddFavoriteAsync(newFavorite);
        if (!ok)
            return (false, "Speichern nicht möglich");

        _logger.LogInformation("Favorit hinzugefügt: {City}/{Country} für User {UserId}", 
            city, country, domainUser.Id);
        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteByIdAsync(
        string applicationUserId, 
        int id)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return (false, "User darf nicht leer sein");

        var domainUser = await _userService.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser == null)
            return (false, "Benutzer nicht gefunden");

        var deleted = await _favoriteRepository.DeleteByIdAsync(domainUser.Id, id);
        if (!deleted)
            return (false, "Favorit nicht gefunden oder nicht gelöscht");

        _logger.LogInformation("Favorit gelöscht: ID {FavoriteId} von User {UserId}", id, domainUser.Id);
        return (true, null);
    }
}
