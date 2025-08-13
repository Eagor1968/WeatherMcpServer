--- Complete source code with proper project structure
source code is here https://github.com/Eagor1968/WeatherMcpServer
--- Instructions for setup and configuration
Dotnet 9.0 should be installsed
to build:
dotnet build
dotnet build -p:DefineConstants="DEBUG"

to run:
dotnet run


logs are contained in 
WeatherMcpServer\logs\server-202508DD.log 

--- Example usage or demo of the working server:

2025-08-13 17:30:08 [Information]  Debug call forecast: "Forecast in Samara,RU: 
Date/Time=13.08.25 19:00, Temperature:23,0, light rain
....
Date/Time=18.08.25 16:00, Temperature:32,3, overcast clouds"

2025-08-13 17:30:09 [Information]  Debug call alerts: "Alerts found in Samara,RU:
1. Местами высокая пожарная опасность event:Пожарная опасность severity:Moderate urgency:Immediate effective:10.08.2025 15:05:00 expires:14.08.2025 19:00:00
2. В ближайшие 1-3 часа местами ожидается гроза event:Гроза severity:Moderate urgency:Immediate effective:13.08.2025 5:06:00 expires:13.08.2025 22:00:00
3. При грозе местами шквалистое усиление ветра порывы 15-20 м/с event:Ветер severity:Moderate urgency:Immediate effective:13.08.2025 5:07:00 expires:13.08.2025 22:00:00
4. Местами ливень event:Дождь severity:Moderate urgency:Immediate effective:13.08.2025 5:07:00 expires:13.08.2025 22:00:00
"

--- Brief documentation of implementation approach

MCP Server is supposed to be used by MCP client integrated into AI especially in Copilot, but Copilot does not work in Russia so I had to create a MCP Client
it is here https://github.com/Eagor1968/McpFullClient
it looks as below
[SERVER] Tools list received:
- get_weather_alerts - Gets weather alerts for the specified city.
- get_random_number - Generates a random number between the specified minimum and maximum values.
- get_weather_forecast - Gets weather forecast for the specified city.
- get_current_weather - Gets current weather conditions for the specified city.

Available tools: get_weather_alerts, get_random_number, get_weather_forecast, get_current_weather
Введите имя инструмента для вызова (или q для выхода):
>

Alerts found in Samara,RU:
1. Местами высокая пожарная опасность event:Пожарная опасность severity:Moderate urgency:Immediate effective:10.08.2025 15:05:00 expires:14.08.2025 19:00:00
2. В ближайшие 1-3 часа местами ожидается гроза event:Гроза severity:Moderate urgency:Immediate effective:13.08.2025 5:06:00 expires:13.08.2025 22:00:00
3. При грозе местами шквалистое усиление ветра порывы 15-20 м/с event:Ветер severity:Moderate urgency:Immediate effective:13.08.2025 5:07:00 expires:13.08.2025 22:00:00
4. Местами ливень event:Дождь severity:Moderate urgency:Immediate effective:13.08.2025 5:07:00 expires:13.08.2025 22:00:00


I implemented GetCurrentWeather and GetWeatherForecast using the OpenWeatherMap API, but this 
API does not allow free usage for alerts, so I implemented GetWeatherAlerts using api.weatherapi.com.