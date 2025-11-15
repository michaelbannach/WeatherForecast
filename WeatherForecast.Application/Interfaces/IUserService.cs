using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<bool> IsSuperUserAsync(string userId);
}