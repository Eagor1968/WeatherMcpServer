using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace WeatherMcpServer.Tools;

public class WeatherTools
{
  private readonly ILogger<WeatherTools> _logger;
  private readonly HttpClient _httpClient;
  private readonly string _weatherMapApiKey;
  private readonly string _weatherApiKey;

  public WeatherTools(
      ILogger<WeatherTools> logger,
      IHttpClientFactory httpClientFactory,
      IConfiguration configuration
    )
  {
    _logger = logger;
    _httpClient = httpClientFactory.CreateClient();
    _weatherMapApiKey = configuration["OpenWeatherMap:ApiKey"] ?? throw new Exception("OpenWeatherMap API key not configured");
    _weatherApiKey = configuration["WeatherAPI:ApiKey"] ?? throw new Exception("WeatherAPI API key not configured");
  }


  [McpServerTool]
  [Description("Gets current weather conditions for the specified city.")]
  public async Task<string> GetCurrentWeather(
      [Description("The city name to get weather for")] string city,
      [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
  {
    try
    {

      string location = getLocation(city, countryCode);

      var url = $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={Uri.EscapeDataString(location)}&" +
                $"appid={_weatherMapApiKey}&units=metric";

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

  [McpServerTool]
  [Description("Gets weather forecast for the specified city.")]
  public async Task<string> GetWeatherForecast(
        [Description("The city name to get forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
  {
    try
    {
      string location = getLocation(city, countryCode);

      var url = $"https://api.openweathermap.org/data/2.5/forecast?" +
                $"q={Uri.EscapeDataString(location)}&" +
                $"appid={_weatherMapApiKey}&units=metric";


      _logger.LogInformation("Requesting forecast for: {Location}", location);

      var response = await _httpClient.GetFromJsonAsync<HourlyForecastResponse>(url);

      if (response == null)
      {
        _logger.LogError("Weather API returned null response");
        return "Forecast data unavailable";
      }

      var result = $"Forecast in {location}: ";
      foreach (var item in response.List)
      {
        result += "\nDate/Time=" + DateTimeOffset.FromUnixTimeSeconds(item.Dt).ToLocalTime().ToString("dd.MM.yy HH:mm")
          + ", Temperature:" + item.Main.Temp.ToString("N1") + ", " + item.Weather[0].Description;
      }

      return result;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting weather for {City}", city);
      return $"Could not retrieve weather: {ex.Message}";
    }
  }

  private static string getLocation(string city, string? countryCode)
  {
    return string.IsNullOrEmpty(countryCode)
        ? city
        : $"{city},{countryCode}";
  }


  [McpServerTool]
  [Description("Gets weather alerts for the specified city.")]
  public async Task<string> GetWeatherAlerts(
        [Description("The city name to get forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)

  {
    try
    {
      string location = getLocation(city, countryCode);
      var url = $"https://api.weatherapi.com/v1/forecast.json" +
                $"?key={_weatherApiKey}&q={Uri.EscapeDataString(location)}&&alerts=yes";

      // Запрос с десериализацией
      var options = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };

      var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(url, options);
      var alerts = response?.alerts?.alert ?? new List<Alert>();
      var result = new StringBuilder();
      if (alerts != null)
      {
        result.AppendLine($"Alerts found in {location}:");
        int index = 1;
        foreach (var alert in alerts)
        {
          result.AppendLine((index++) + ". " + alert.desc + " event:" + alert.@event + " severity:"
            + alert.severity + " urgency:" + alert.urgency + " effective:" + alert.effective + " expires:" + alert.expires);
        }
      }
      else
      {
        result.AppendLine($"No alerts found in {location}");
      }

      return result.ToString();

    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting alerts for {City}", city);
      return $"Could not retrieve alerts: {ex.Message}";
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

  public record GeoItem
  (
    string name,
    float lat,
    float lon,
    string country
  );

}