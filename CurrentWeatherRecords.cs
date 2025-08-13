namespace WeatherMcpServer
{

  // Models for deserializing OpenWeatherMap response
  public record OpenWeatherResponse(
      CurrentWeatherMainData Main,
      WeatherDescription[] Weather);

  public record CurrentWeatherMainData(
      float Temp,
      int Humidity);

  public record WeatherDescription(
      string Description);


}
