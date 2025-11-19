using Microsoft.EntityFrameworkCore;
using WeatherForecast.Infrastructure.Data;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly AppDbContext _appDbContext;

    public FavoriteRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;
    public Task<List<Favorite>> GetFavoritesAsync(string userId) =>
        _appDbContext.Favorites
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Id)
            .ToListAsync();


    public async Task<bool> AddFavoriteAsync(Favorite favorite)
    {
        _appDbContext.Favorites.Add(favorite);
        return await _appDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteByIdAsync(string userId, int id)
    {
      var entity =  await _appDbContext.Favorites.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
      if(entity == null) 
          return false;
      
      _appDbContext.Favorites.Remove(entity);


      var changes = await _appDbContext.SaveChangesAsync();
      return changes > 0;
    }
    
    public Task<int> CountFavoritesAsync(string userId)
    {
        return  _appDbContext.Favorites
            .CountAsync(f => f.UserId == userId);
    }
    
    public Task<bool>AlreadyExistsAsync(string userId, string city,  string country) =>
    _appDbContext.Favorites.AnyAsync(f => f.UserId == userId && f.City == city && f.Country == country);
}