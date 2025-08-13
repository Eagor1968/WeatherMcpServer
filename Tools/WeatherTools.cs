using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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
      
      var location = string.IsNullOrEmpty(countryCode)
          ? city
          : $"{city},{countryCode}";

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
      
      var location = string.IsNullOrEmpty(countryCode)
          ? city
          : $"{city},{countryCode}";

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

  //public async Task<List<Alert>> GetWeatherAlerts(string location)
  //{
  //  // Формируем URL
  //  var url = $"https://api.weatherapi.com/v1/forecast.json" +
  //            $"?key={_weatherApiKey}&q={Uri.EscapeDataString(location)}&days=3&alerts=yes&lang=ru";

  //  // Запрос с десериализацией
  //  var options = new JsonSerializerOptions
  //  {
  //    PropertyNameCaseInsensitive = true
  //  };

  //  var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(url, options);

  //  return response?.alerts?.alert ?? new List<Alert>();
  //}


  [McpServerTool]
  [Description("Gets weather alerts for the specified city.")]
  public async Task<string> GetWeatherAlerts_openweathermap(
        [Description("The city name to get forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
  {
    try
    {
      var geoItem = await GetGeoItem(city, countryCode);

      var location = string.IsNullOrEmpty(countryCode)
          ? city
          : $"{city},{countryCode}";


      var url = $"https://api.openweathermap.org/data/2.5/onecall?" +
                $"lat={geoItem.lat.ToString(CultureInfo.InvariantCulture)}&lon={geoItem.lon.ToString(CultureInfo.InvariantCulture)}&" +
                $"appid={_weatherMapApiKey}&units=metric";


      _logger.LogInformation("Requesting alerts for: {Location}", location);

      var response = await _httpClient.GetFromJsonAsync<OneCallResponse>(url);

      if (response == null)
      {
        _logger.LogError("Weather API returned null response");
        return "Forecast data unavailable";
      }

      var result = $"Alerts in {location}: ";
      if (response.alerts.Length == 0)
      {
        result += " absent";
      }
      else
      {
        foreach (var alert in response.alerts)
        {
          result += $"\n event:{alert.@event} start:{alert.start} end:{alert.end} description:{alert.description}";
        }
      }


      return result;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting alerts for {City}", city);
      return $"Could not retrieve alerts: {ex.Message}";
    }
  }


  [McpServerTool]
  [Description("Gets weather alerts for the specified city.")]
  public async Task<string> OpenWeatherMap_GetWeatherAlerts(
        [Description("The city name to get forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
  {
    try
    {
      var geoItem = await GetGeoItem(city, countryCode);
      
      var location = string.IsNullOrEmpty(countryCode)
          ? city
          : $"{city},{countryCode}";
      

      var url = $"https://api.openweathermap.org/data/2.5/onecall?" +
                $"lat={geoItem.lat.ToString(CultureInfo.InvariantCulture)}&lon={geoItem.lon.ToString(CultureInfo.InvariantCulture)}&" +
                $"appid={_weatherMapApiKey}&units=metric";


      _logger.LogInformation("Requesting alerts for: {Location}", location);

      var response = await _httpClient.GetFromJsonAsync<OneCallResponse>(url);

      if (response == null)
      {
        _logger.LogError("Weather API returned null response");
        return "Forecast data unavailable";
      }

      var result = $"Alerts in {location}: ";
      if(response.alerts.Length == 0)
      {
        result += " absent";
      } else
      {
        foreach(var alert in response.alerts)
        {
          result += $"\n event:{alert.@event} start:{alert.start} end:{alert.end} description:{alert.description}";
        }
      }


        return result;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting alerts for {City}", city);
      return $"Could not retrieve alerts: {ex.Message}";
    }
  }


  public async Task<GeoItem> GetGeoItem(
        [Description("The city name to get forecast for")] string city,
        [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null)
  {
    var location = string.IsNullOrEmpty(countryCode)
        ? city
        : $"{city},{countryCode}";

    var url = $"https://api.openweathermap.org/geo/1.0/direct?" +
              $"q={Uri.EscapeDataString(location)}&" +
              $"appid={_weatherMapApiKey}&units=metric";

    _logger.LogInformation("Requesting Geo codes for: {Location}", location);

    var response = await _httpClient.GetFromJsonAsync<GeoItem[]>(url);

    if (response == null || response.Length == 0)
    {
      _logger.LogError("Geo codes API returned null response");
      return null;
    }


    return response[0];


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