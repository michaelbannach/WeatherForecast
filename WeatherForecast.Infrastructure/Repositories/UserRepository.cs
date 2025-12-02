using Microsoft.EntityFrameworkCore;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Data;

namespace WeatherForecast.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByApplicationUserIdAsync(string applicationUserId) =>
        _db.AppUsers
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);

    public async Task<User> CreateForApplicationUserAsync(string applicationUserId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = applicationUserId
            
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }
}