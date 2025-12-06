using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        private readonly IConfiguration _configuration;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            ILogger<AuthService> logger,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<(bool success, string? error, string? token)> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("LoginAsync: User {Email} not found", email);
                    return (false, "LoginAsync, FindByEmailAsync: Invalid Login", null);
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, password);
                if (!passwordValid)
                {
                    _logger.LogWarning("LoginAsync: Invalid password for {Email}", email);
                    return (false, "LoginAsync, CheckPassword: Invalid Login", null);
                }

                var roles = await _userManager.GetRolesAsync(user);

                var token = GenerateJwtToken(user, roles);
                _logger.LogInformation("LoginAsync, GetRolesAsync: User {Email} logged in, token issued", email);

                return (true, null, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginAsync: Unexpected error for {Email}", email);
                return (false, "LoginAsync:Internal Server error", null);
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
        
        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var m) ? m : 60;

            if (string.IsNullOrWhiteSpace(key))
            {
                
                throw new InvalidOperationException("Jwt:Key is missing or empty in configuration");
            }

            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("Jwt:Issuer or Jwt:Audience is missing in configuration");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email ?? "")
            };

            foreach (var role in roles ?? Array.Empty<string>())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

 
    }
}
