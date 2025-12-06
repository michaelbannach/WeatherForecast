namespace WeatherForecast.Application.Interfaces;

public interface IAuthService
{
    Task<(bool success, string? error, string? token)> LoginAsync(string email, string password);
    Task<(bool success, string? error, string? appUserId, Guid? domainUserId)> RegisterAsync(
        string email, string password, string firstName, string lastName, string? role = "User");
    Task LogoutAsync();
}
