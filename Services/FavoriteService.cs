using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Services;

public class FavoriteService 
{
   private readonly IFavoriteRepository _favoriteRepository;

   public FavoriteService(IFavoriteRepository favoriteRepository) =>
   
      _favoriteRepository = favoriteRepository;
   
   //Methoden um Input zu normalisieren für konsistentes Format
   private static string NormCity(string s) => (s ?? "").Trim();
   private static string NormCountry(string s) => (s ?? "").Trim().ToUpper();
   
   public Task<List<Favorite>> GetFavoritesAsync(string userId) => 
      _favoriteRepository.GetFavoritesAsync(userId);

   public async Task<(bool added, string? error)> AddFavoriteAsync(string userId, Favorite favorite)
   {
      if (string.IsNullOrWhiteSpace(userId))
         return (false, "Unbekannter Benutzer");
      
      var city = NormCity(favorite.City);
      var country = NormCountry(favorite.Country);
      
      if(string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
         return (false, "Stadt und Land dürfen nicht leer sein");

      var count = await _favoriteRepository.CountFavoritesAsync(userId);

      if (count >= 5)
         return (false, "Maximal 5 erlaubt");

      var exists = await _favoriteRepository.AllreadyExistAsync(userId, city, country);
      if(exists)
         return (false, "Diese Stadt ist bereits gespeichert");
      
      var ok = await _favoriteRepository.AddAsync((new Favorite { UserId = userId,City = city, Country = country }));
      return (ok, ok ? null : "Speichern nicht möglich");
   }
}