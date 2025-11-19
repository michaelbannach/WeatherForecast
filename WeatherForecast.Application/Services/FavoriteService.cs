using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Dtos;
using WeatherForecast.Application.Mapping;
using WeatherForecast.Domain.Models;
using WeatherForecast.Application.Interfaces;

namespace WeatherForecast.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserService _userService;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(IFavoriteRepository favoriteRepository,IUserService userService, ILogger<FavoriteService> logger)
    {
        _favoriteRepository = favoriteRepository;
        _userService = userService;
        _logger = logger;
    }

    private static string NormCity(string s) => (s ?? "").Trim();
    private static string NormCountry(string s) => (s ?? "").Trim().ToUpper();

    public async Task<List<FavoriteDto>> GetFavoritesAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("GetFavoritesAsync wurde mit leerer UserId aufgerufen");
            throw new ArgumentException("UserId darf nicht leer sein");
        }

        _logger.LogInformation("Lade Favoriten für User {UserId}", userId);
        var favorites = await _favoriteRepository.GetFavoritesAsync(userId);
        _logger.LogInformation("{Count} Favoriten für User {UserId} geladen", favorites.Count, userId);

        return favorites.Select(EntityToDtoMapper.ToDto).ToList();
    }

    public async Task<(bool added, string? error)> AddFavoriteAsync(string userId, FavoriteDto favoriteDto)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("AddFavoriteAsync: unbekannter Benutzer");
            return (false, "Unbekannter Benutzer");
        }

        if (!await _userService.IsSuperUserAsync(userId))
        {
            _logger.LogWarning("User {UserId} ist kein SuperUser", userId);
            return (false, "Keine Berechtigung zum Speichern");
        }

        var city = NormCity(favoriteDto.City);
        var country = NormCountry(favoriteDto.Country);

        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
        {
            _logger.LogWarning("AddFavoriteAsync: Stadt oder Land leer - City: '{City}', Country: '{Country}'", city, country);
            return (false, "Stadt und Land dürfen nicht leer sein");
        }

        var exists = await _favoriteRepository.AlreadyExistsAsync(userId, city, country);
        if (exists)
        {
            _logger.LogInformation("AddFavoriteAsync: Favorit existiert bereits für User {UserId} - {City}, {Country}", userId, city, country);
            return (false, "Diese Stadt ist bereits gespeichert");
        }

        var count = await _favoriteRepository.CountFavoritesAsync(userId);
        if (count >= 5)
        {
            _logger.LogInformation("AddFavoriteAsync: Maximal 5 Favoriten für User {UserId} erreicht", userId);
            return (false, "Maximal 5 erlaubt");
        }

        _logger.LogInformation("Füge Favorit hinzu: User {UserId}, Stadt {City}, Land {Country}", userId, city, country);

        var ok = await _favoriteRepository.AddFavoriteAsync(
            new Favorite { UserId = userId, City = city, Country = country });

        if (!ok)
        {
            _logger.LogError("AddFavoriteAsync: Fehler beim Speichern des Favoriten für User {UserId}", userId);
            return (false, "Speichern nicht möglich");
        }

        return (true, null);
    }
    
    public async Task<(bool deleted, string? error)> DeleteByIdAsync(string userId, int id)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("DeleteByIdAsync mit leerer UserId aufgerufen");
            return (false, "User darf nicht leer sein");
        }

        _logger.LogInformation("Lösche Favorit mit Id {FavoriteId} für User {UserId}", id, userId);
       
        var deleted = await _favoriteRepository.DeleteByIdAsync(userId, id);

        if (!deleted)
        {
            
            var message = $"Favorit mit Id {id} für User {userId} wurde nicht gefunden oder konnte nicht gelöscht werden.";
            _logger.LogWarning("DeleteByIdAsync: {Message}", message);
            return (false, message);
        }
            

        _logger.LogInformation("Favorit mit Id {FavoriteId} erfolgreich gelöscht.", id);
        return (true, null);
    }
}
