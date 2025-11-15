using Microsoft.AspNetCore.Identity;

using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }
    public async Task<bool> IsSuperUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, "SuperUser");
    }
    
    
}