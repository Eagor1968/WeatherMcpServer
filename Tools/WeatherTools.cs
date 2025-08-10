using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
  private readonly ILogger<WeatherTools> _logger;
  private readonly HttpClient _httpClient;

  // Используйте конфигурацию вместо хардкода API-ключа
  private readonly string _apiKey;

  public WeatherTools(
      ILogger<WeatherTools> logger,
      IHttpClientFactory httpClientFactory,
      IConfiguration configuration
    )
  {
    _logger = logger;
    _httpClient = httpClientFactory.CreateClient();
    _apiKey = configuration["OpenWeatherMap:ApiKey"]
        ?? throw new Exception("OpenWeatherMap API key not configured");
  }

  [McpServerTool]
  [Description("Describes random weather in the provided city.")]
  public string GetCityWeather([Description("Name of the city to return weather for")] string city)
  {
    _logger.LogDebug("Called GetCityWeather for city: {City}", city);

    var weatherTypes = "sunny,cloudy,rainy,stormy,snowy".Split(',');
    var selectedWeather = weatherTypes[Random.Shared.Next(weatherTypes.Length)];

    return $"The weather in {city} is {selectedWeather}.";
  }

  [McpServerTool]
  [Description("Gets current weather conditions for the specified city.")]
  public async Task<string> GetCurrentWeather(
      [Description("The city name to get weather for")] string city,
      [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
  {
    try
    {
      // Формируем запрос с учетом страны
      var location = string.IsNullOrEmpty(countryCode)
          ? city
          : $"{city},{countryCode}";

      var url = $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={Uri.EscapeDataString(location)}&" +
                $"appid={_apiKey}&units=metric";

      _logger.LogInformation("Requesting weather for: {Location}", location);

      var response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);

      if (response == null)
      {
        _logger.LogError("Weather API returned null response");
        return "Weather data unavailable";
      }

      var result = $"Current weather in {city}: " +
                   $"{response.Main.Temp}°C, " +
                   $"{response.Weather[0].Description}";

      _logger.LogDebug("Weather response: {@Response}", response);

      return result;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting weather for {City}", city);
      return $"Could not retrieve weather: {ex.Message}";
    }
  }

  // Модели для десериализации ответа OpenWeatherMap
  private record OpenWeatherResponse(
      MainData Main,
      WeatherDescription[] Weather);

  private record MainData(
      float Temp,
      int Humidity);

  private record WeatherDescription(
      string Description);
}