using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);


// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "logs/server-.log",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 1_000_000,
        retainedFileCountLimit: 7,
        rollOnFileSizeLimit: true)
    .MinimumLevel.Debug()
    .CreateLogger();

// Подключаем Serilog к DI
builder.Logging.ClearProviders(); // Убираем Console и др.
builder.Logging.AddSerilog();     // Добавляем Serilog

builder.Services.AddHttpClient();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();

var host  = builder.Build();


// Определяем путь к каталогу, где лежит сервер (не клиента!)
var serverDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
var appSettingsPath = Path.Combine(serverDirectory!, "appsettings.json");

// Добавляем конфигурацию из файла в каталоге сервера
builder.Configuration.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);
await host.RunAsync();