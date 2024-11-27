using Lokalise.ReviewComments.App.Services;
using Lokalise.ReviewComments.Business;
using Lokalise.ReviewComments.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

// Register services
builder.Services.AddReviewCommentsServices(builder.Configuration);

var host = builder.Build();

// Get service and run
var app = host.Services.GetRequiredService<IApp>();
app.Run();


var lokalise = new LokaliseClient();
var comments = await lokalise.GetComments();

await host.RunAsync();