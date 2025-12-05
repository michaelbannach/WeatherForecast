using System.Text.Json;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.External.OpenWeatherMap;

namespace WeatherForecast.Infrastructure.Services;

public class WeatherService : IWeatherService
{
    private readonly ILogger<WeatherService> _logger;
    private readonly HttpClient _http;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOpt = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Helper to show error messages from OpenWeatherMap API
    private sealed class OwmError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    public WeatherService(HttpClient http, IConfiguration config, ILogger<WeatherService> logger)
    {
        _http = http;
        _logger = logger;

        // WeatherMapUrl from Variable -- No Hardcoding
        var baseUrl = config["OpenWeatherMap:BaseUrl"]
            ?? throw new InvalidOperationException("OpenWeatherMap:BaseUrl is required");

        _http.BaseAddress = new Uri(baseUrl.Trim());

        _apiKey = config["OpenWeatherMap:ApiKey"]
            ?? throw new InvalidOperationException("OpenWeatherMap:ApiKey is required");

        _apiKey = _apiKey.Trim();

        _logger.LogInformation("WeatherService initialized. BaseUrl: {@BaseUrl}", _http.BaseAddress);
    }

    public async Task<(Weather? data, string? error)> GetWeatherAsync(string city, string country)
    {
        _logger.LogInformation("Start WeatherRequest for City '{City}', Country '{Country}'", city, country);

        var (normalizedCity, normalizedCountry, error) = Validate(city, country);

        if (error is not null)
        {
            _logger.LogWarning("Validation error: {Error}", error);
            return (null, error);
        }

        var url = $"weather?q={normalizedCity},{normalizedCountry}&appid={_apiKey}&units=metric&lang=de";

        _logger.LogInformation("OpenWeatherMap-API: {Url}", url);

        var (owm, apiError) = await GetJsonAsync<CompleteWeatherResponse>(url);

        if (apiError != null)
        {
            _logger.LogWarning("API-error: {ApiError}", apiError);
            return (null, apiError);
        }

        if (owm == null)
        {
            _logger.LogWarning("No Weatherdata");
            return (null, "No Weatherdata");
        }

        // OWM → Domain Model (Weather)
        var weather = owm.ToWeatherEntity(normalizedCity, normalizedCountry);

        _logger.LogInformation("Wetterdata mapped successfully for {City},{Country}",
            normalizedCity, normalizedCountry);

        return (weather, null);
    }

   
    private async Task<(List<Forecast>? data, string? error)> GetForecastAsync(
        string city,
        string country,
        int days)
    {
        _logger.LogInformation(
            "Start Forecast-Request {Days} Days for City '{City}', Country '{Country}'",
            days, city, country);

        var (normalizedCity, normalizedCountry, error) = Validate(city, country);

        if (error is not null)
        {
            _logger.LogWarning("Validation error Forecast: {Error}", error);
            return (null, error);
        }

        var url = $"forecast?q={normalizedCity},{normalizedCountry}&appid={_apiKey}&units=metric&lang=de";

        _logger.LogInformation("OpenWeatherMap-API: {Url}", url);

        var (owm, apiError) = await GetJsonAsync<CompleteForecastResponse>(url);

        if (apiError != null)
        {
            _logger.LogWarning("API-error Forecast: {ApiError}", apiError);
            return (null, apiError);
        }

        if (owm == null)
        {
            _logger.LogWarning("No Forecast-Data received");
            return (null, "No Forecast-Data received");
        }

        
        var forecasts = owm.ToForecastEntities();

        
        if (days > 0)
            forecasts = forecasts.Take(days).ToList();

        _logger.LogInformation("Forecast-Data mapped successfully for {City},{Country}",
            normalizedCity, normalizedCountry);

        return (forecasts, null);
    }

    public Task<(List<Forecast>? data, string? error)> GetThreeDayForecastAsync(string city, string country)
        => GetForecastAsync(city, country, 3);

    public Task<(List<Forecast>? data, string? error)> GetFiveDayForecastAsync(string city, string country)
        => GetForecastAsync(city, country, 5);

    private async Task<(T? data, string? error)> GetJsonAsync<T>(string url)
    {
        try
        {
            _logger.LogDebug("Send HTTP-Request: {RequestUrl}", url);

            var resp = await _http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var msg = $"{(int)resp.StatusCode} {resp.ReasonPhrase}";
                var owm = JsonSerializer.Deserialize<OwmError>(body, JsonOpt);

                if (!string.IsNullOrWhiteSpace(owm?.Message))
                    msg += $": {owm.Message}";

                _logger.LogWarning("HTTP-Status error. {Msg} | Body: {Body}", msg, body);
                return (default, msg);
            }

            _logger.LogDebug("Successful API-response received");
            return (JsonSerializer.Deserialize<T>(body, JsonOpt), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error HTTP-Request: {Url}", url);
            return (default, ex.Message);
        }
    }

    private static (string City, string Country, string? Error) Validate(string city, string country)
    {
        var normalizedCity = city?.Trim() ?? string.Empty;
        var normalizedCountry = country?.Trim().ToUpperInvariant() ?? string.Empty;

        
        normalizedCountry = new string(normalizedCountry.Where(char.IsLetter).ToArray());

        if (normalizedCountry.Length < 2)
            normalizedCountry = normalizedCountry[..2];

        if (string.IsNullOrWhiteSpace(normalizedCity))
            return (normalizedCity, normalizedCountry, "Specify a City.");

        if (normalizedCountry.Length != 2)
            return (normalizedCity, normalizedCountry, "Country code must be 2 digits, e.g., DE.");

        return (normalizedCity, normalizedCountry, null);
    }
}
