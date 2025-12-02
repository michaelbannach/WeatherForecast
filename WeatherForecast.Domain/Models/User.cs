using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    [Required] public string ApplicationUserId { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}