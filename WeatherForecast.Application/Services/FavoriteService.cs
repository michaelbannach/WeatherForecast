using WeatherForecast.Application.Interfaces;
using WeatherForecast.Application.Dtos;
using WeatherForecast.Application.Mapping;
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

    private static string NormCity(string s) => (s ?? "").Trim();
    private static string NormCountry(string s) => (s ?? "").Trim().ToUpper();

    public async Task<List<FavoriteDto>> GetFavoritesAsync(string applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new ArgumentException("UserId darf nicht leer sein");

        var domainUser = await _userService.GetOrCreateDomainUserAsync(applicationUserId);

        var favorites = await _favoriteRepository.GetFavoritesAsync(domainUser.Id);
        return favorites.Select(EntityToDtoMapper.ToDto).ToList();
    }

    public async Task<(bool added, string? error)> AddFavoriteAsync(string applicationUserId, FavoriteDto favoriteDto)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return (false, "Unbekannter Benutzer");

        if (!await _userService.IsSuperUserAsync(applicationUserId))
            return (false, "Keine Berechtigung zum Speichern");

        var domainUser = await _userService.GetOrCreateDomainUserAsync(applicationUserId);

        var city = NormCity(favoriteDto.City);
        var country = NormCountry(favoriteDto.Country);

        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
            return (false, "Stadt und Land dürfen nicht leer sein");

        var exists = await _favoriteRepository.AlreadyExistsAsync(domainUser.Id, city, country);
        if (exists)
            return (false, "Diese Stadt ist bereits gespeichert");

        var count = await _favoriteRepository.CountFavoritesAsync(domainUser.Id);
        if (count >= 5)
            return (false, "Maximal 5 erlaubt");

        var ok = await _favoriteRepository.AddFavoriteAsync(
            new Favorite { UserId = domainUser.Id, City = city, Country = country });

        if (!ok)
            return (false, "Speichern nicht möglich");

        return (true, null);
    }

    public async Task<(bool deleted, string? error)> DeleteByIdAsync(string applicationUserId, int id)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return (false, "User darf nicht leer sein");

        var domainUser = await _userService.GetOrCreateDomainUserAsync(applicationUserId);

        var deleted = await _favoriteRepository.DeleteByIdAsync(domainUser.Id, id);

        if (!deleted)
            return (false, $"Favorit mit Id {id} wurde nicht gefunden oder konnte nicht gelöscht werden.");

        return (true, null);
    }
}

