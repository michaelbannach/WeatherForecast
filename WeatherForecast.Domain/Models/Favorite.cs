using System.ComponentModel.DataAnnotations;



namespace WeatherForecast.Domain.Models;

public class Favorite
{
    public int Id { get; set; }
    
    public string City { get; set; } = string.Empty;
    
    
    public string Country { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    
}