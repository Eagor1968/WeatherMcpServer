using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WeatherMcpServer.Tools;

public static class WeatherDebugRunner
{
  public static async Task RunDebugCallsAsync(IHost host)
  {
    try
    {
      var weatherTools = host.Services.GetRequiredService<WeatherTools>();

      var currentWeather = await weatherTools.GetCurrentWeather("Samara", "RU");
      Log.Information("Debug call current weather: {@Result}", currentWeather);

      var forecast = await weatherTools.GetWeatherForecast("Samara", "RU");
      Log.Information("Debug call forecast: {@Result}", forecast);

      var alerts = await weatherTools.GetWeatherAlerts("Samara", "RU");
      Log.Information("Debug call alerts: {@Result}", alerts);

    }
    catch (Exception ex)
    {
      Log.Error(ex, "Error during debug call to GetCurrentWeather");
    }
  }
}