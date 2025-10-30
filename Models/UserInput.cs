using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Models;

public class UserInput
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2)]
    public string Country { get; set; } = string.Empty;

    public string UserId { get; set; } = null!;

    public ApplicationUser ApplicationUser { get; set; } = null!;

}