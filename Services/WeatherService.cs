using System.Text.Json;
using Microsoft.Extensions.Logging;
using WeatherForecast.Dtos;
using WeatherForecast.Interfaces;


namespace WeatherForecast.Services;

public sealed class WeatherService : IWeatherService

{
    private readonly ILogger<WeatherService> _logger;
    //HttpClient-Instanz -> HTTP-Anfragen an externe API
    private readonly HttpClient _http;

    //In dieser Variable wird der API-Key gespeichert
    private readonly string _apiKey;

    //Dadurch wird API-Antwort CaseInsensitiv
    private static readonly JsonSerializerOptions JsonOpt = new() { PropertyNameCaseInsensitive = true };

    //Hilfsklasse um Fehlerantworten der OpenWeatherMap API abzubilden
    private sealed class OwmError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }
    
    public WeatherService(HttpClient http, IConfiguration config, ILogger<WeatherService> logger)
    {
        _http = http;
        _logger = logger; // Logger speichern
        _http.BaseAddress = new Uri(config["OpenWeatherMap:BaseUrl"]);
        _apiKey = (config["OpenWeatherMap:ApiKey"] ??
            throw new InvalidOperationException("OpenWeatherMap:ApiKey is required")).Trim();

        _logger.LogInformation("WeatherService initialisiert. BaseUrl: {BaseUrl}", _http.BaseAddress);
    }

    public async Task<(CompleteWeatherResponse? data, string? error)> GetWeatherAsync(string city, string country)
    {
        _logger.LogInformation("Starte Wetterabfrage für Stadt '{City}', Land '{Country}'", city, country);

        if (!Validate(city, country, out var c, out var co, out var error))
        {
            _logger.LogWarning("Validierungsfehler: {Error}", error);
            return (null, error);
        }

        var url = $"weather?q={c},{co}&appid={_apiKey}&units=metric&lang=de";
        _logger.LogInformation("Rufe OpenWeatherMap-API auf: {Url}", url);

        var (data, apiError) = await GetJsonAsync<CompleteWeatherResponse>(url);

        if (apiError != null)
            _logger.LogWarning("API-Fehler: {ApiError}", apiError);

        if (data != null)
            _logger.LogInformation("Wetterdaten erfolgreich geladen: {Data}", data);

        return (data, apiError);
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
}
