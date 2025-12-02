using Microsoft.AspNetCore.Identity;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Models;

namespace WeatherForecast.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;

    public UserService(UserManager<ApplicationUser> userManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<bool> IsSuperUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, "SuperUser");
    }

    public async Task<User> GetOrCreateDomainUserAsync(string applicationUserId)
    {
       
        var domainUser = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);
        if (domainUser != null)
            return domainUser;

        
        var identityUser = await _userManager.FindByIdAsync(applicationUserId)
                           ?? throw new InvalidOperationException("ApplicationUser nicht gefunden");

        return await _userRepository.CreateForApplicationUserAsync(identityUser.Id);
    }
}