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

        //Validation of Input
        if (!Validate(city, country, out var c, out var co, out var error))
        {
            _logger.LogWarning("Validierungsfehler: {Error}", error);
            return (null, error);
        }

        var url = $"weather?q={c},{co}&appid={_apiKey}&units=metric&lang=de";
        
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
            return(null,"Keine Wetterdaten empfangen");
        }

        var dto = owm.ToAppDto(city, country);
        
        _logger.LogInformation("Wetterdaten erfolgreich gemappt für {City},{Country}", city, country);
      
        return(dto, null);
        
    }
   
    private static bool Validate(string city, string country, out string c, out string co, out string? error)
    {
        c = (city ?? "").Trim();
        co = (country ?? "").Trim().ToUpperInvariant();
        co = new string(co.Where(char.IsLetter).ToArray());
        if (co.Length >= 2) co = co[..2];

        if (string.IsNullOrWhiteSpace(c))
        {
            error = "Bitte eine Stadt angeben.";
            return false;
        }
        if (co.Length != 2)
        {
            error = "Ländercode muss 2-stellig sein (z. B. DE).";
            return false;
        }
        error = null;
        return true;
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
    
    public async Task<(List<ForecastDto>? data, string? error)> GetForecastAsync(string city, string country, int days)
    {
        _logger.LogInformation("Starte Forecast-Abfrage ({Days} Tage) für Stadt '{City}', Land '{Country}'", days, city, country);

        if (!Validate(city, country, out var c, out var co, out var error))
        {
            _logger.LogWarning("Validierungsfehler (Forecast): {Error}", error);
            return (null, error);
        }

        var url = $"forecast?q={c},{co}&appid={_apiKey}&units=metric&lang=de";
        _logger.LogInformation("Rufe OpenWeatherMap-Forecast-API auf: {Url}", url);

        var (forecastResponse, apiError) = await GetJsonAsync<CompleteForecastResponse>(url);

        if (apiError != null)
        {
            _logger.LogWarning("API-Fehler bei Forecast: {ApiError}", apiError);
            return (null, apiError);
        }

        if (forecastResponse == null)
        {
            _logger.LogWarning("Keine Forecastdaten empfangen");
            return (null, "Keine Forecastdaten empfangen");
        }

        var forecastDtos = forecastResponse.ToForecastDtos()
            .OrderBy(x => x.Date)
            .Take(days)
            .ToList();

        _logger.LogInformation("Forecast mit {ForecastCount} Tagen erfolgreich gemappt ({City},{Country})", forecastDtos.Count, city, country);

        return (forecastDtos, null);
    }

    public Task<(List<ForecastDto>? data, string? error)> GetThreeDayForecastAsync(string city, string country)
        => GetForecastAsync(city, country, 3);

    public Task<(List<ForecastDto>? data, string? error)> GetFiveDayForecastAsync(string city, string country)
        => GetForecastAsync(city, country, 5);
}
