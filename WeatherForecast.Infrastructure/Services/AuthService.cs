using Microsoft.AspNetCore.Identity;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Models;

namespace WeatherForecast.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            ILogger<AuthService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _logger = logger;
        }
        public async Task<(bool success, string? error)> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("LoginAsync: User {Email} logged in successfully", email);
                    return (true, null);
                }
                _logger.LogWarning("LoginAsync: Failed login attempt for {Email}", email);
                return (false, "Ungültige Anmeldedaten");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginAsync: Unexpected error for {Email}", email);
                return (false, "Interner Serverfehler");
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("LogoutAsync: User logged out");
        }
        public async Task<(bool success, string? error, string? appUserId, Guid? domainUserId)> RegisterAsync(
            string email, string password, string firstName, string lastName, string? role = "User")
        {
            try
            {
                
                var appUser = new ApplicationUser { UserName = email, Email = email };
                var identityResult = await _userManager.CreateAsync(appUser, password);

                if (!identityResult.Succeeded)
                {
                    var errorText = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("RegisterAsync: Failed to create Identity-User. Errors: {Errors}", errorText);
                    return (false, errorText, null, null);
                }

                
                var domainUser = new User
                {
                    ApplicationUserId = appUser.Id,
                    FirstName = firstName,
                    LastName = lastName
                };

                var (userSuccess, userError, createdDomainUser) = await _userService.CreateUserAsync(domainUser);
                if (!userSuccess || createdDomainUser == null)
                {
                    _logger.LogError("RegisterAsync: Failed to create domain user. Error: {Error}", userError);
                    await _userManager.DeleteAsync(appUser); // Cleanup
                    return (false, userError ?? "Failed to create domain user", null, null);
                }

                
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("RegisterAsync: Failed to create role {Role}", role);
                        return (false, $"Failed to create role: {role}", null, null);
                    }
                }

                var roleResult2 = await _userManager.AddToRoleAsync(appUser, role);
                if (!roleResult2.Succeeded)
                {
                    _logger.LogError("RegisterAsync: Failed to assign role {Role} to user {UserId}", role, appUser.Id);
                    return (false, $"Failed to assign role: {role}", null, null);
                }

                _logger.LogInformation("RegisterAsync: User created successfully. AppUserId: {AppUserId}, DomainUserId: {DomainUserId}", 
                    appUser.Id, createdDomainUser.Id);

                return (true, null, appUser.Id, createdDomainUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterAsync: Unexpected error");
                return (false, "Internal server error", null, null);
            }
        }
        
        
 
    }
}
