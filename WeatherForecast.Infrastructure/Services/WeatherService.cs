using System.Text.Json;

using WeatherForecast.Application.Dtos;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Infrastructure.External.OpenWeatherMap;

namespace WeatherForecast.Infrastructure.Services;

public class WeatherService : IWeatherService

{
    private readonly ILogger<WeatherService> _logger;
    
    private readonly HttpClient _http;

   
    private readonly string _apiKey;

    
    private static readonly JsonSerializerOptions JsonOpt = new() { PropertyNameCaseInsensitive = true };

    //Helper to show errormessages from OpenWeatherMap API
    private sealed class OwmError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }
    
    // IConfiguration is injected by ASP.NET Core and contains values from the appsettings files.
    public WeatherService(HttpClient http, IConfiguration config, ILogger<WeatherService> logger)
    {
        _http = http;
        _logger = logger;
        
        //WeatherMapUrl from Variable --> No Hardcoding
        var baseUrl = config["OpenWeatherMap:BaseUrl"]
        ?? throw new InvalidOperationException("OpenWeatherMap:BaseUrl is required");
        
        _http.BaseAddress = new Uri(baseUrl.Trim());
        
        _apiKey = (config["OpenWeatherMap:ApiKey"] ??
            throw new InvalidOperationException("OpenWeatherMap:ApiKey is required")).Trim();

        _logger.LogInformation("WeatherService initialisiert. BaseUrl: {BaseUrl}", _http.BaseAddress);
    }

    public async Task<(WeatherDto? data, string? error)> GetWeatherAsync(string city, string country)
    {
        _logger.LogInformation("Starte Wetterabfrage für Stadt '{City}', Land '{Country}'", city, country);

        var (normalizedCity, normalizedCountry, error) = Validate(city, country);

        if (error is not null)
        {
            _logger.LogWarning("Validierungsfehler: {Error}", error);
            return (null, error);
        }

        var url = $"weather?q={normalizedCity},{normalizedCountry}&appid={_apiKey}&units=metric&lang=de";
        
        _logger.LogInformation("Rufe OpenWeatherMap-API auf: {Url}", url);

        var (owm, apiError) = await GetJsonAsync<CompleteWeatherResponse>(url);

        if (apiError != null)
        {
            _logger.LogWarning("API-Fehler: {ApiError}", apiError);
            return (null, apiError);
        }

        if (owm == null)
        {
            _logger.LogWarning("Keine Wetterdaten empfangen");
            return (null, "Keine Wetterdaten empfangen");
        }

        // für Fallback nehmen wir die normalisierte Stadt/Country
        var dto = owm.ToAppDto(normalizedCity, normalizedCountry);
        
        _logger.LogInformation("Wetterdaten erfolgreich gemappt für {City},{Country}", normalizedCity, normalizedCountry);
      
        return (dto, null);
    }
   
    

    private async Task<(T? data, string? error)> GetJsonAsync<T>(string url)
    {
        try
        {
            _logger.LogDebug("Sende HTTP-Request: {RequestUrl}", url);
            var resp = await _http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var msg = $"{(int)resp.StatusCode} {resp.ReasonPhrase}";
                var owm = JsonSerializer.Deserialize<OwmError>(body, JsonOpt);
                if (!string.IsNullOrWhiteSpace(owm?.Message))
                    msg += $" ({owm.Message})";
                _logger.LogWarning("HTTP-Status nicht erfolgreich: {Msg}, Body: {Body}", msg, body);
                return (default, msg);
            }

            _logger.LogDebug("Erfolgreiche API-Antwort erhalten");
            return (JsonSerializer.Deserialize<T>(body, JsonOpt), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim HTTP-Request: {Url}", url);
            return (default, ex.Message);
        }
    }
    private static (string City, string Country, string? Error) Validate(string city, string country)
    {
        var normalizedCity = (city ?? string.Empty).Trim();
        var normalizedCountry = (country ?? string.Empty).Trim().ToUpperInvariant();

        // nur Buchstaben erlauben
        normalizedCountry = new string(normalizedCountry.Where(char.IsLetter).ToArray());

        if (normalizedCountry.Length >= 2)
            normalizedCountry = normalizedCountry[..2];

        if (string.IsNullOrWhiteSpace(normalizedCity))
            return ("", "", "Bitte eine Stadt angeben.");

        if (normalizedCountry.Length != 2)
            return ("", "", "Ländercode muss 2-stellig sein (z. B. DE).");

        return (normalizedCity, normalizedCountry, null);
    }
    
    public async Task<(List<ForecastDto>? data, string? error)> GetForecastAsync(string city, string country, int days)
    {
        _logger.LogInformation("Starte Forecast-Abfrage ({Days} Tage) für Stadt '{City}', Land '{Country}'", days, city, country);

        var (normalizedCity, normalizedCountry, error) = Validate(city, country);

        if (error is not null)
        {
            _logger.LogWarning("Validierungsfehler (Forecast): {Error}", error);
            return (null, error);
        }

        var url = $"forecast?q={normalizedCity},{normalizedCountry}&appid={_apiKey}&units=metric&lang=de";

        _logger.LogInformation("Rufe OpenWeatherMap-API (Forecast) auf: {Url}", url);

        var (owm, apiError) = await GetJsonAsync<CompleteForecastResponse>(url);

        if (apiError != null)
        {
            _logger.LogWarning("API-Fehler (Forecast): {ApiError}", apiError);
            return (null, apiError);
        }

        if (owm == null)
        {
            _logger.LogWarning("Keine Forecast-Daten empfangen");
            return (null, "Keine Forecast-Daten empfangen");
        }

        var dtos = owm.ToForecastDtos();

        _logger.LogInformation("Forecast-Daten erfolgreich gemappt für {City},{Country}", normalizedCity, normalizedCountry);

        // ggf. days begrenzen (3/5-Tage-Use-Cases)
        if (days > 0)
            dtos = dtos.Take(days).ToList();

        return (dtos, null);
    }

    public Task<(List<ForecastDto>? data, string? error)> GetThreeDayForecastAsync(string city, string country)
        => GetForecastAsync(city, country, 3);

    public Task<(List<ForecastDto>? data, string? error)> GetFiveDayForecastAsync(string city, string country)
        => GetForecastAsync(city, country, 5);
}
