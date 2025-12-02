using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetByApplicationUserIdAsync(string applicationUserId);
    
    Task<(bool success, string? error, User? user)> CreateUserAsync(User user);
    
  
}