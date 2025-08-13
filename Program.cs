using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning) // tun off information logs from  HttpClient
    .WriteTo.File(
        path: "logs/server-.log",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 1_000_000,
        retainedFileCountLimit: 7,
        rollOnFileSizeLimit: true)
    .CreateLogger();

// Connect Serilog to DI
builder.Logging.ClearProviders(); // Remove Console and other default providers
builder.Logging.AddSerilog();     // Add Serilog

builder.Services.AddHttpClient();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

builder.Services.AddSingleton<WeatherTools>();

// Determine the path to the directory where the server is located (not the client!)
var serverDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
var appSettingsPath = Path.Combine(serverDirectory!, "appsettings.json");

// Add configuration from the file in the server directory
builder.Configuration.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

var host = builder.Build();

#if DEBUG
// === DEBUG CALLS: ===
await WeatherDebugRunner.RunDebugCallsAsync(host);
#endif

await host.RunAsync();
