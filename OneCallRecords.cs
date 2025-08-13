namespace WeatherMcpServer
{
  public record OneCallResponse(
      float lat,
      float lon,
      string timezone,
      int timezone_offset,
      CurrentWeather current,
      Minutely[]? minutely,
      Hourly[]? hourly,
      Daily[]? daily,
      Alert[]? alerts
  );

  public record CurrentWeather(
      long dt,
      long sunrise,
      long sunset,
      float temp,
      float feels_like,
      int pressure,
      int humidity,
      float dew_point,
      float uvi,
      int clouds,
      int visibility,
      float wind_speed,
      int wind_deg,
      float wind_gust,
      WeatherDescription[] weather
  );

  public record Minutely(
      long dt,
      float precipitation
  );

  public record Hourly(
      long dt,
      float temp,
      float feels_like,
      int pressure,
      int humidity,
      float dew_point,
      float uvi,
      int clouds,
      int visibility,
      float wind_speed,
      int wind_deg,
      float wind_gust,
      WeatherDescription[] weather,
      float pop
  );

  public record Daily(
      long dt,
      long sunrise,
      long sunset,
      long moonrise,
      long moonset,
      float moon_phase,
      string? summary,
      DailyTemp temp,
      DailyFeelsLike feels_like,
      int pressure,
      int humidity,
      float dew_point,
      float wind_speed,
      int wind_deg,
      float wind_gust,
      WeatherDescription[] weather,
      int clouds,
      float pop,
      float? rain,
      float uvi
  );

  public record DailyTemp(
      float day,
      float min,
      float max,
      float night,
      float eve,
      float morn
  );

  public record DailyFeelsLike(
      float day,
      float night,
      float eve,
      float morn
  );

  public record WeatherDescription(
      int id,
      string main,
      string description,
      string icon
  );

  public record Alert(
      string sender_name,
      string @event,
      long start,
      long end,
      string description,
      string[] tags
  );
}
