using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByApplicationUserIdAsync(string applicationUserId);
    Task<User> CreateForApplicationUserAsync(string applicationUserId);
}