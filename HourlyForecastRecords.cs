using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WeatherMcpServer
{
    // Основной ответ
    public record HourlyForecastResponse(
        string Cod,
        double Message,
        int Cnt,
        List<ForecastItem> List,
        CityInfo City);

    // Элемент прогноза (по часам)
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

    // Температура и давление
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

    // Погодные условия
    public record WeatherInfo(
        int Id,
        string Main,
        string Description,
        string Icon);

    // Облачность
    public record CloudsInfo(
        int All);

    // Ветер
    public record WindInfo(
        double Speed,
        int Deg,
        double? Gust);

    // Дождь (за последний час)
    public record RainInfo(
        [property: JsonPropertyName("1h")]
    double OneHour);

    // Снег (за последний час)
    public record SnowInfo(
        [property: JsonPropertyName("1h")]
    double OneHour);

    // Системная информация (часть дня)
    public record SysInfo(
        string Pod);

    // Информация о городе
    public record CityInfo(
        int Id,
        string Name,
        Coordinates Coord,
        string Country,
        int Timezone,
        long Sunrise,
        long Sunset);

    // Координаты
    public record Coordinates(
        double Lat,
        double Lon);
}
