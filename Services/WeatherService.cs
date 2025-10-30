using System.Text.Json;
using WeatherForecast.Dtos;
using  WeatherForecast.Models;

namespace WeatherForecast.Services;

public sealed class WeatherService
{
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

    public WeatherService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _http.BaseAddress = new Uri(config["https://api.openweathermap.org/data/2.5"]);

        //Liest API-Key aus der appsettings.Development.json -> Prüft ob Key nicht null ist -> sonst Exception
        _apiKey = (config["OpenWeatherMap:ApiKey"] ??
                   throw new InvalidOperationException("OpenWeatherMap:ApiKey is required")).Trim();
    }

    //Abfrage des aktuellen Wetters
    public async Task<(CompleteWeatherResponse? data, string? error)> GetWeatherAsync(string city, string country)
    {
        if (!Validate(city, country, out var c, out var co, out var error))
            return (null, error);

        var url = $"weather?q={c},{co}&appid={_apiKey}&units=metric&lang=de";
        return await GetJsonAsync<CompleteWeatherResponse>(url);
    }

    //Helpermethode um korrekte Eingabe zu garantieren und hilfreiche Fehlermeldung zu bekommen
    private static bool Validate(string city, string country, out string c, out string co, out string? error)
    {
        c = (city ?? "").Trim();
        co = (country ?? "").Trim().ToUpperInvariant();


        co = new string(co.Where(char.IsLetter).ToArray());


        if (co.Length >= 2)
            co = co[..2];

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
            var resp = await _http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var msg = $"{(int)resp.StatusCode} {resp.ReasonPhrase}";
                var owm = JsonSerializer.Deserialize<OwmError>(body, JsonOpt);
                if (!string.IsNullOrWhiteSpace(owm?.Message))
                    msg += $" ({owm.Message})";
                return (default, msg);
            }

            return (JsonSerializer.Deserialize<T>(body, JsonOpt), null);
        }
        catch (Exception ex)
        {
            return (default, ex.Message);
        }
    }
}