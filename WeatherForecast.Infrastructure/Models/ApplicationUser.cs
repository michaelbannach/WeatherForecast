using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}