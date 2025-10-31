using Microsoft.EntityFrameworkCore;
using WeatherForecast.Data;
using WeatherForecast.Models;

namespace WeatherForecast.Repositories;

public class FavoriteRepository
{
    private readonly AppDbContext _appDbContext;

    public Task<List<Favorite>> GetFavoritesAsync(string userId) =>
        _appDbContext.UserInputs
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Id)
            .Take(5)
            .ToListAsync();


    public async Task<bool> AddAsync(Favorite favorite)
    {
        _appDbContext.UserInputs.Add(favorite);
        return await _appDbContext.SaveChangesAsync() > 0;
    }
}