using Lokalise.ReviewComments.Business;
using Lokalise.ReviewComments.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.AddReviewCommentsServices();

var host = builder.Build();

// Get service and run
var app = host.Services.GetRequiredService<IApp>();
app.Run();

await host.RunAsync();