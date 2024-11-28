using Lokalise.ReviewComments.App.Services;
using Lokalise.ReviewComments.Business;
using Lokalise.ReviewComments.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

// Configure Serilog to write logs to a JSON file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/log.json", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new SerilogLoggerProvider(Log.Logger));

// Register services
builder.Services.AddReviewCommentsServices(builder.Configuration);

var host = builder.Build();

// Get service and run
var app = host.Services.GetRequiredService<IApp>();
app.Run();


var lokalise = new LokaliseClient();
var comments = await lokalise.GetComments();

await host.RunAsync();