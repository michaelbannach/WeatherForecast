namespace WeatherForecast.Domain.Models;

public class Forecast
{
  
    public DateTime Date { get; set; }
    
    public double TempMin { get; set; }
    
    public double TempMax { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public string Icon { get; set; } = string.Empty;
}