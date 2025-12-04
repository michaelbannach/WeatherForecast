namespace WeatherForecast.Domain.Models;

public class Weather
{
    public string City { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
    
    public double Temp { get; set; }
    
    public double FeelsLike { get; set; }
    
    public double TempMin { get; set; }
    
    public double TempMax { get; set; }
    
    public double Humidity { get; set; }
    
    public double WindSpeed { get; set; }
    
    public double Sunrise { get; set; }
    
    public double Sunset { get; set; }
    
    public string Summary { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Icon { get; set; } = string.Empty;
}