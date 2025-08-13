using System.Text.Json.Serialization;

namespace WeatherMcpServer
{
    public record HourlyForecastResponse(
        string Cod,
        double Message,
        int Cnt,
        List<ForecastItem> List,
        CityInfo City);

    public record ForecastItem(
        long Dt,
        MainData Main,
        List<WeatherInfo> Weather,
        CloudsInfo Clouds,
        WindInfo Wind,
        RainInfo? Rain,
        SnowInfo? Snow,
        int Visibility,
        double Pop,
        SysInfo Sys,
        string DtTxt);


    public record MainData(
        double Temp,
        double FeelsLike,
        double TempMin,
        double TempMax,
        int Pressure,
        int SeaLevel,
        int GrndLevel,
        int Humidity,
        double TempKf);

    public record WeatherInfo(
        int Id,
        string Main,
        string Description,
        string Icon);

    public record CloudsInfo(
        int All);

    public record WindInfo(
        double Speed,
        int Deg,
        double? Gust);

    public record RainInfo(
        [property: JsonPropertyName("1h")]
    double OneHour);

    public record SnowInfo(
        [property: JsonPropertyName("1h")]
    double OneHour);

    public record SysInfo(
        string Pod);

    public record CityInfo(
        int Id,
        string Name,
        Coordinates Coord,
        string Country,
        int Timezone,
        long Sunrise,
        long Sunset);

    public record Coordinates(
        double Lat,
        double Lon);
}
