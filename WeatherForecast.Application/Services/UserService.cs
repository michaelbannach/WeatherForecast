using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;


namespace WeatherForecast.Application.Services;

public class UserService : IUserService
{
    
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    } 
    public async Task<User?> GetByApplicationUserIdAsync(string applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
        {
            _logger.LogWarning("GetByApplicationUserIdAsync: Invalid applicationUserId provided");
            return null;
        }

        try
        {
            var user = await _userRepository.GetByApplicationUserIdAsync(applicationUserId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetByApplicationUserIdAsync: Error retrieving user for {ApplicationUserId}", applicationUserId);
            return null;
        }
    }

    public async Task<(bool success, string? error, User? user)> CreateUserAsync(User user)
    {
        
        if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            return (false, "Vor- und Nachname erforderlich", null);

        try
        {
            var existingUser = await _userRepository.GetByApplicationUserIdAsync(user.ApplicationUserId);
            if (existingUser != null)
                return (false, "User existiert bereits", null);

            user.Id = Guid.NewGuid();  
        
            var createdUser = await _userRepository.CreateAsync(user);
            return (true, null, createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return (false, "Error creating user", null);
        }
    }
}