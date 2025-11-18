using System.ComponentModel.DataAnnotations;



namespace WeatherForecast.Domain.Models;

public class Favorite
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2)]
    public string Country { get; set; } = string.Empty;

    public string UserId { get; set; } = null!;

   

}