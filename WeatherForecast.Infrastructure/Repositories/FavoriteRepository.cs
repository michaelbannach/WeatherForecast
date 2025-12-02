using Microsoft.EntityFrameworkCore;
using WeatherForecast.Infrastructure.Data;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly AppDbContext _appDbContext;

    public FavoriteRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

    public Task<List<Favorite>> GetFavoritesAsync(Guid userId) =>
        _appDbContext.Favorites
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Id)
            .ToListAsync();

    public async Task<bool> AddFavoriteAsync(Favorite favorite)
    {
        _appDbContext.Favorites.Add(favorite);
        return await _appDbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid userId, int id)
    {
        var entity = await _appDbContext.Favorites
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (entity == null)
            return false;

        _appDbContext.Favorites.Remove(entity);
        return await _appDbContext.SaveChangesAsync() > 0;
    }

    public Task<int> CountFavoritesAsync(Guid userId) =>
        _appDbContext.Favorites.CountAsync(f => f.UserId == userId);

    public Task<bool> AlreadyExistsAsync(Guid userId, string city, string country) =>
        _appDbContext.Favorites.AnyAsync(f =>
            f.UserId == userId && f.City == city && f.Country == country);
}
