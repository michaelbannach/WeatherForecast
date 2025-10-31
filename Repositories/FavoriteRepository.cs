using Microsoft.EntityFrameworkCore;
using WeatherForecast.Data;
using WeatherForecast.Models;

namespace WeatherForecast.Repositories;

public class FavoriteRepository
{
    private readonly AppDbContext _appDbContext;

    public FavoriteRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;
    public Task<List<Favorite>> GetFavoritesAsync(string userId) =>
        _appDbContext.Favorites
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Id)
            .Take(5)
            .ToListAsync();


    public async Task<bool> AddAsync(Favorite favorite)
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
}