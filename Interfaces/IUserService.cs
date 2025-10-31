using WeatherForecast.Models;

namespace WeatherForecast.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<bool> IsSuperUserAsync(string userId);
}