using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WeatherForecast.Infrastructure.Services;
using Xunit;

namespace WeatherForecast.Application.Tests.Services;

public class WeatherServiceTests
{
    private HttpClient CreateHttpClient(HttpStatusCode statusCode, string content)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });

        return new HttpClient(handlerMock.Object);
    }

    private IConfiguration CreateConfig()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "OpenWeatherMapBaseUrl", "https://api.test.com/" },
            { "OpenWeatherMapApiKey", "test-key" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    private WeatherService CreateService(HttpClient client)
    {
        // Test-Konfiguration bereitstellen
        var settings = new Dictionary<string, string?>
        {
            ["OpenWeatherMap:BaseUrl"] = "https://api.openweathermap.org/data/2.5/",
            ["OpenWeatherMap:ApiKey"]  = "TEST_API_KEY"
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();

        var loggerMock = new Mock<ILogger<WeatherService>>();

        return new WeatherService(client, config, loggerMock.Object);
    }

    [Fact]
    public async Task GetWeatherAsync_returns_error_if_city_missing()
    {
        var http = CreateHttpClient(HttpStatusCode.OK, "{}");
        var sut = CreateService(http);

        var (data, error) = await sut.GetWeatherAsync("", "DE");

        Assert.Null(data);
        Assert.Equal("Specify a City.", error);
    }

    [Fact]
    public async Task GetWeatherAsync_returns_error_if_http_not_success()
    {
        var http = CreateHttpClient(HttpStatusCode.BadRequest, "{\"message\":\"Bad request\"}");
        var sut = CreateService(http);

        var (data, error) = await sut.GetWeatherAsync("Berlin", "DE");

        Assert.Null(data);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task GetWeatherAsync_returns_error_if_http_throws_exception()
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Boom"));

        var http = new HttpClient(handlerMock.Object);
        var sut = CreateService(http);

        var (data, error) = await sut.GetWeatherAsync("Berlin", "DE");

        Assert.Null(data);
        Assert.Equal("Boom", error);
    }

    [Fact]
    public async Task GetWeatherAsync_success_returns_weather()
    {
        var json = @"{
          ""weather"": [{ ""main"": ""Clouds"", ""description"": ""broken clouds"", ""icon"": ""04d"" }],
          ""main"": { ""temp"": 20.5, ""feels_like"": 21.0, ""temp_min"": 19.0, ""temp_max"": 22.0, ""humidity"": 60 },
          ""wind"": { ""speed"": 5.0 },
          ""sys"": { ""sunrise"": 1712131200, ""sunset"": 1712178000 }
        }";

        var http = CreateHttpClient(HttpStatusCode.OK, json);
        var sut = CreateService(http);

        var (data, error) = await sut.GetWeatherAsync("Berlin", "DE");

        Assert.Null(error);
        Assert.NotNull(data);
        Assert.Equal("Berlin", data!.City);
        Assert.Equal("DE", data.Country);
    }

    [Fact]
    public async Task GetThreeDayForecastAsync_returns_error_if_city_missing()
    {
        var http = CreateHttpClient(HttpStatusCode.OK, "{}");
        var sut = CreateService(http);

        var (data, error) = await sut.GetThreeDayForecastAsync("", "DE");

        Assert.Null(data);
        Assert.Equal("Specify a City.", error);
    }

    [Fact]
    public async Task GetThreeDayForecastAsync_returns_error_if_http_not_success()
    {
        var http = CreateHttpClient(HttpStatusCode.BadRequest, "{\"message\":\"Bad request\"}");
        var sut = CreateService(http);

        var (data, error) = await sut.GetThreeDayForecastAsync("Berlin", "DE");

        Assert.Null(data);
        Assert.NotNull(error);
    }
}
