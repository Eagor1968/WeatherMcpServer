namespace WeatherMcpServer
{
  public record WeatherApiResponse(
      AlertContainer alerts
  );

  public record AlertContainer(
      List<Alert> alert
  );

  public record Alert(
      string headline,
      string msgtype,
      string severity,
      string urgency,
      string areas,
      string category,
      string @event,
      DateTime effective,
      DateTime expires,
      string desc
  );


}
