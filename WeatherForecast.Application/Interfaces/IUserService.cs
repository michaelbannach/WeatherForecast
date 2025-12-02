using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces;

public interface IUserService
{
    
    Task<bool> IsSuperUserAsync(string userId);
    
    Task<User> GetOrCreateDomainUserAsync(string applicationUserId);
}